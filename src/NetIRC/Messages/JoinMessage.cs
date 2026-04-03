using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a JOIN message from server or client.
    /// </summary>
    public class JoinMessage : IRCMessage, IServerMessage, IClientMessage
    {
        private readonly string channels;
        private readonly string keys = string.Empty;

        /// <summary>
        /// Gets the nickname that joined (server variant).
        /// </summary>
        public string Nick { get; }

        /// <summary>
        /// Gets the channel that was joined.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public JoinMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channels">Comma-separated channel list.</param>
        /// <param name="keys">Optional comma-separated keys.</param>
        public JoinMessage(string channels, string keys = "")
        {
            this.channels = channels;
            this.keys = keys;
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channels">Channel names to join.</param>
        public JoinMessage(params string[] channels)
        {
            this.channels = string.Join(",", channels);
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channelsWithKeys">Channel-to-key map.</param>
        public JoinMessage(Dictionary<string, string> channelsWithKeys)
        {
            channels = string.Join(",", channelsWithKeys.Keys);
            keys = string.Join(",", channelsWithKeys.Values);
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "JOIN", channels, keys };
    }
}
