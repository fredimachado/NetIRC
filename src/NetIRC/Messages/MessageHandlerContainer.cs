using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NetIRC.Tests")]

namespace NetIRC.Messages
{
    internal class MessageHandlerContainer
    {
        private readonly Client client;

        private readonly Dictionary<string, MessageHandler> defaultMessageHandlers = new Dictionary<string, MessageHandler>();
        private readonly Dictionary<string, MessageHandler> customMessageHandlers = new Dictionary<string, MessageHandler>();

        private const string HandlerNameSuffix = "Handler";

        internal MessageHandlerContainer(Client client)
            : this(client, typeof(MessageHandlerContainer).Assembly)
        {}

        internal MessageHandlerContainer(Client client, Assembly assembly)
        {
            this.client = client;

            RegisterDefaultMessageHandlers(assembly);
        }

        public void RegisterCustomMessageHandler(Type type)
        {
            if (!IsCustomMessageHandler(type))
            {
                throw new InvalidOperationException($"{type.Name} must implement IMessageHandler<TServerMessage>.");
            }
            Type messageType = GetMessageType(type);

            if (!IsMessageConstructorValid(messageType))
            {
                throw new InvalidOperationException($"{messageType.Name} must have a constructor with exactly one parameter of type {nameof(ParsedIRCMessage)}.");
            }

            var handler = new MessageHandler(type, messageType);
            var command = GetCommand(handler);

            customMessageHandlers.Add(command, handler);
        }

        public void RegisterCustomMessageHandlers(Assembly assembly)
        {
            var customHandlers = assembly
                .GetExportedTypes()
                .Where(t => IsCustomMessageHandler(t) && IsMessageConstructorValid(GetMessageType(t)));

            foreach (var handler in customHandlers)
            {
                RegisterCustomMessageHandler(handler);
            }
        }

        private static Type GetMessageType(Type type)
        {
            var messageType = type.GetInterfaces()
                .First() // Gets IMessageHandler<TServerMessage>
                .GetGenericArguments()
                .First(); // Gets TServerMessage
            return messageType;
        }

        private bool IsMessageConstructorValid(Type messageType)
        {
            return messageType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    types: new[] { typeof(ParsedIRCMessage) },
                    modifiers: null) != null;
        }

        private void RegisterDefaultMessageHandlers(Assembly assembly)
        {
            var handlers = assembly
                .GetExportedTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(MessageHandler<>))
                .SelectMany(t => t.GetInterfaces(), (parent, child) => new MessageHandler(parent, child.GetGenericArguments().First()))
                .ToArray();

            foreach (var handler in handlers)
            {
                string command = GetCommand(handler);

                defaultMessageHandlers.Add(command, handler);
            }
        }

        private bool IsCustomMessageHandler(Type t)
        {
            return !t.IsInterface && !t.IsAbstract && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(CustomMessageHandler<>);
        }

        private string GetCommand(MessageHandler handler)
        {
            var command = handler.HandlerType.GetCustomAttribute<CommandAttribute>()?.Command;

            // Fallback to class name's prefix
            if (string.IsNullOrWhiteSpace(command))
            {
                command = handler.HandlerType.Name.Replace(HandlerNameSuffix, string.Empty);
            }

            return command.ToUpper();
        }

        internal async Task<object> HandleAsync(ParsedIRCMessage parsedIRCMessage)
        {
            if (customMessageHandlers.TryGetValue(parsedIRCMessage.Command, out var customMessageHandler))
            {
                var result = await InvokeHandler<ICustomHandler>(parsedIRCMessage, customMessageHandler)
                    .ConfigureAwait(false);

                // Prevent running default (internal) message handler
                // This might have side-effects, especially for privmsg and channel related commands,
                // since Queries and Channels collection states rely on those messages in Client
                if (result.Handled)
                {
                    return result;
                }
            }

            if (!defaultMessageHandlers.TryGetValue(parsedIRCMessage.Command, out var messageHandler))
            {
                return null;
            }

            return await InvokeHandler<object>(parsedIRCMessage, messageHandler)
                .ConfigureAwait(false);
        }

        internal async Task<T> InvokeHandler<T>(ParsedIRCMessage parsedIRCMessage, MessageHandler messageHandler)
            where T : class
        {
            var message = Activator.CreateInstance(messageHandler.MessageType, new object[] { parsedIRCMessage });
            var handler = Activator.CreateInstance(messageHandler.HandlerType);

            messageHandler.HandlerType
                .GetProperty("Message")
                .SetValue(handler, message);

            await ((Task)messageHandler.HandleMethod.Invoke(handler, new object[] { message, client }))
                .ConfigureAwait(false);

            return handler as T;
        }
    }

    internal class MessageHandler
    {
        public Type HandlerType { get; }
        public Type MessageType { get; }
        public MethodInfo HandleMethod { get; }
        public MessageHandler(Type handlerType, Type messageType)
        {
            HandlerType = handlerType;
            MessageType = messageType;
            HandleMethod = handlerType
                .GetMethods()
                .Single(m => m.Name == "HandleAsync");
        }
    }
}
