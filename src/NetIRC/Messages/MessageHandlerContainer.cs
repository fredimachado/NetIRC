using NetIRC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NetIRC.Tests")]
namespace NetIRC
{
    internal class MessageHandlerContainer
    {
        private readonly Client client;

        private readonly Dictionary<string, MessageHandler> defaultMessageHandlers = new Dictionary<string, MessageHandler>();
        private readonly Dictionary<string, MessageHandler> customMessageHandlers = new Dictionary<string, MessageHandler>();

        private const string HandlerNameSuffix = "Handler";

        internal MessageHandlerContainer(Client client)
        {
            this.client = client;

            RegisterDefaultMessageHandlers();
        }

        public void RegisterCustomMessageHandler(Type type)
        {
            if (!IsMessageHandler(type))
            {
                throw new InvalidOperationException($"{type.Name} must implement IMessageHandler<TServerMessage>.");
            }

            var messageType = type.GetInterfaces()
                .First() // Gets IMessageHandler<TServerMessage>
                .GetGenericArguments()
                .First(); // Gets TServerMessage

            if (!IsMessageConstructorValid(messageType))
            {
                throw new ArgumentException($"{type.Name} must have a constructor with exactly one parameter of type {nameof(ParsedIRCMessage)}.", nameof(type));
            }

            var handler = new MessageHandler(type, messageType);
            var command = GetCommand(handler);

            customMessageHandlers.Add(command, handler);
        }

        private bool IsMessageConstructorValid(Type messageType)
        {
            return messageType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    types: new[] { typeof(ParsedIRCMessage) },
                    modifiers: null) != null;
        }

        private void RegisterDefaultMessageHandlers()
        {
            var handlers = typeof(MessageHandlerContainer).Assembly
                .GetExportedTypes()
                .Where(t => IsMessageHandler(t))
                .SelectMany(t => t.GetInterfaces(), (parent, child) => new MessageHandler(parent, child.GetGenericArguments().First()))
                .ToArray();

            foreach (var handler in handlers)
            {
                string command = GetCommand(handler);

                defaultMessageHandlers.Add(command, handler);
            }
        }

        private static bool IsMessageHandler(Type t)
        {
            return t.GetInterfaces()
                .Where(i => i.IsGenericType && !t.IsAbstract)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
        }

        private static string GetCommand(MessageHandler handler)
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
