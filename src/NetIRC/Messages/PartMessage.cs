using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PartMessage : IRCMessage, IServerMessage, IClientMessage
    {
        private readonly string channels;

        public string Nick { get; }
        public string Channel { get; }

        public PartMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        public PartMessage(string channels)
        {
            this.channels = channels;
        }

        public PartMessage(params string[] channels)
            : this(string.Join(",", channels))
        {
        }

        public IEnumerable<string> Tokens => new[] { "PART", channels };
    }
}
