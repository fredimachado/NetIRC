namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a KICK message from the server.
    /// </summary>
    public class KickMessage : IRCMessage, IServerMessage
    {
        /// <summary>
        /// Gets the nickname that issued the kick.
        /// </summary>
        public string KickedBy { get; }

        /// <summary>
        /// Gets the channel where the kick occurred.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets the nickname that was kicked.
        /// </summary>
        public string Nick { get; }

        /// <summary>
        /// Gets the kick comment.
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="KickMessage"/>.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public KickMessage(ParsedIRCMessage parsedMessage)
        {
            KickedBy = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
            Nick = parsedMessage.Parameters[1];
            Comment = parsedMessage.Trailing;
        }
    }
}
