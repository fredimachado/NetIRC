using GravyIrc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GravyIrc.Messages
{
    public abstract class IrcMessage
    {
        private static readonly Dictionary<string, Type> ServerMessageTypes;

        static IrcMessage()
        {
            var interfaceType = typeof(IServerMessage);
            ServerMessageTypes = typeof(IrcMessage).Assembly
                .GetTypes()
                .Where(t => interfaceType.IsAssignableFrom(t))
                .Where(t => t.HasCommand())
                .ToDictionary(t => t.GetCommand(), t => t);
        }

        public static IServerMessage Create(ParsedIrcMessage parsedMessage)
        {
            var messageType = ServerMessageTypes.ContainsKey(parsedMessage.Command)
                ? ServerMessageTypes[parsedMessage.Command]
                : null;

            return messageType != null
                ? Activator.CreateInstance(messageType, new object[] { parsedMessage }) as IServerMessage
                : null;
        }

        public override string ToString()
        {
            var clientMessage = this as IClientMessage;

            if (clientMessage == null)
            {
                return base.ToString();
            }

            var tokens = clientMessage.Tokens.ToArray();

            if (!tokens.Any())
            {
                return string.Empty;
            }

            return string.Join(" ", tokens.Select(t => t == tokens.LastOrDefault() && t.Contains(' ') ? $":{t}" : t)).Trim();
        }
    }
}
