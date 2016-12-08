using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public IRCMessage()
        {
        }

        public virtual IEnumerable<string> Tokens => Enumerable.Empty<string>();

        public abstract void TriggerEvent(EventHub eventHub);

        public override string ToString()
        {
            var tokens = Tokens.ToArray();

            if (tokens.Length == 0)
            {
                return string.Empty;
            }

            var lastIndex = tokens.Length - 1;

            var sb = new StringBuilder();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i == lastIndex && tokens[i].Contains(" "))
                {
                    sb.Append(':');
                }

                sb.Append(tokens[i]);

                if (i < lastIndex)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}
