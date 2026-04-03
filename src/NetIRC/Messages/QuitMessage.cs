using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a QUIT message from server or client.
    /// </summary>
    public class QuitMessage : IRCMessage, IServerMessage, IClientMessage
    {
        /// <summary>
        /// Gets the nickname that quit (server variant).
        /// </summary>
        public string Nick { get; }

        /// <summary>
        /// Gets the quit message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public QuitMessage(ParsedIRCMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Message = parsedMessage.Trailing;
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="message">Quit message text.</param>
        public QuitMessage(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "QUIT", Message };
    }
}
