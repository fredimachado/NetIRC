using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class JoinMessage : IRCMessage, IServerMessage, IClientMessage
    {
        private readonly string channels;
        private readonly string keys = string.Empty;

        public string Nick { get; }
        public string Channel { get; }

        public JoinMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        public JoinMessage(string channels, string keys = "")
        {
            this.channels = channels;
            this.keys = keys;
        }

        public JoinMessage(params string[] channels)
        {
            this.channels = string.Join(",", channels);
        }

        public JoinMessage(Dictionary<string, string> channelsWithKeys)
        {
            channels = string.Join(",", channelsWithKeys.Keys);
            keys = string.Join(",", channelsWithKeys.Values);
        }

        public IEnumerable<string> Tokens => new[] { "JOIN", channels, keys };
    }
}
