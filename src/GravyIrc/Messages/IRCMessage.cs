using GravyIrc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GravyIrc.Messages
{
    public abstract class IRCMessage
    {
        private static readonly Dictionary<string, Type> ServerMessageTypes;

        static IRCMessage()
        {
            var interfaceType = typeof(IServerMessage);
            ServerMessageTypes = typeof(IRCMessage).Assembly
                .GetTypes()
                .Where(t => interfaceType.IsAssignableFrom(t))
                .Where(t => t.HasAction())
                .ToDictionary(t => t.GetAction(), t => t);
        }

        public static IServerMessage Create(ParsedIRCMessage parsedMessage)
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
