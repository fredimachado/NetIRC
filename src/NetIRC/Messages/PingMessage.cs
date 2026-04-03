namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a server PING command.
    /// </summary>
    public class PingMessage : IRCMessage, IServerMessage
    {
        /// <summary>
        /// Gets the ping target/token.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PingMessage"/>.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public PingMessage(ParsedIRCMessage parsedMessage)
        {
            Target = parsedMessage.Trailing;
        }
    }
}
