using System;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public abstract class ServerMessage : IRCMessage
    {
        protected readonly ParsedIRCMessage parsedMessage;

        private static readonly Dictionary<string, Type> messageMapping;

        public ParsedIRCMessage ParsedIRCMessage => parsedMessage;

        static ServerMessage()
        {
            messageMapping = new Dictionary<string, Type>
            {
                { "PING", typeof(PingMessage) },
                { "PRIVMSG", typeof(PrivMsgMessage) },
                { "NOTICE", typeof(NoticeMessage) },
                { "NICK", typeof(NickMessage) },
            };
        }

        // Not using reflection yet because of .NET Standard
        // Will be updated when .NET Standard 2.0 gets released
        public static ServerMessage Create(ParsedIRCMessage parsedMessage)
        {
            var messageType = messageMapping.ContainsKey(parsedMessage.Command)
                ? messageMapping[parsedMessage.Command]
                : typeof(DefaultIRCMessage);

            return (ServerMessage)Activator.CreateInstance(messageType, parsedMessage);
        }

        public abstract void TriggerEvent(EventHub eventHub);

        public ServerMessage(ParsedIRCMessage parsedMessage)
        {
            this.parsedMessage = parsedMessage;
        }

        public ServerMessage()
        {
        }
    }
}
