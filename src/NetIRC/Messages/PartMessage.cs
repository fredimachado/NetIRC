using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a PART message from server or client.
    /// </summary>
    public class PartMessage : IRCMessage, IServerMessage, IClientMessage
    {
        private readonly string channels;

        /// <summary>
        /// Gets the nickname that left (server variant).
        /// </summary>
        public string Nick { get; }

        /// <summary>
        /// Gets the channel that was left (server variant).
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public PartMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channels">Comma-separated channel list.</param>
        public PartMessage(string channels)
        {
            this.channels = channels;
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="channels">Channel names.</param>
        public PartMessage(params string[] channels)
            : this(string.Join(",", channels))
        {
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "PART", channels };
    }
}
