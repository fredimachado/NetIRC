namespace NetIRC.Messages
{
    /// <summary>
    /// Represents server numeric 001 (welcome) message.
    /// </summary>
    public class RplWelcomeMessage : IRCMessage, IServerMessage
    {
        /// <summary>
        /// Gets the welcome text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="RplWelcomeMessage"/>.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public RplWelcomeMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
