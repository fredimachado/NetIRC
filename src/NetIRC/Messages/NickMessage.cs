using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a NICK message from server or client.
    /// </summary>
    public class NickMessage : IRCMessage, IServerMessage, IClientMessage
    {
        /// <summary>
        /// Gets the old nickname (server variant).
        /// </summary>
        public string OldNick { get; }

        /// <summary>
        /// Gets the new nickname.
        /// </summary>
        public string NewNick { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public NickMessage(ParsedIRCMessage parsedMessage)
        {
            OldNick = parsedMessage.Prefix.From;
            NewNick = parsedMessage.Parameters[0];
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="newNick">New nickname to apply.</param>
        public NickMessage(string newNick)
        {
            NewNick = newNick;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "NICK", NewNick };
    }
}
