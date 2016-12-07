using System;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public abstract class IRCMessage
    {
        protected readonly ParsedIRCMessage parsedMessage;

        private static readonly Dictionary<string, Type> messageMapping;

        public ParsedIRCMessage ParsedIRCMessage => parsedMessage;

        static IRCMessage()
        {
            messageMapping = new Dictionary<string, Type>
            {
                { "PING", typeof(PingCommand) },
                { "PRIVMSG", typeof(PrivMsgMessage) },
            };
        }

        // Not using reflection yet because of .NET Standard
        // Will be updated when .NET Standard 2.0 gets released
        public static IRCMessage Create(ParsedIRCMessage parsedMessage)
        {
            var messageType = messageMapping.ContainsKey(parsedMessage.Command)
                ? messageMapping[parsedMessage.Command]
                : typeof(DefaultIRCMessage);

            return (IRCMessage)Activator.CreateInstance(messageType, parsedMessage);
        }

        public IRCMessage(ParsedIRCMessage parsedMessage)
        {
            this.parsedMessage = parsedMessage;
        }

        public abstract void TriggerEvent(EventHub eventHub);
    }
}
