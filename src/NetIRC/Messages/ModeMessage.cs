using System.Collections.Generic;
using System.Linq;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a MODE message from server or client.
    /// </summary>
    public class ModeMessage : IRCMessage, IServerMessage, IClientMessage
    {
        /// <summary>
        /// Gets the source prefix (server variant).
        /// </summary>
        public IRCPrefix Prefix { get; }

        /// <summary>
        /// Gets the mode target (user or channel).
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets mode flags string.
        /// </summary>
        public string Modes { get; }

        /// <summary>
        /// Gets extra mode parameters when present.
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public ModeMessage(ParsedIRCMessage parsedMessage)
        {
            Prefix = parsedMessage.Prefix;
            Target = parsedMessage.Parameters[0];
            Modes = parsedMessage.Parameters[1] ?? parsedMessage.Trailing;
            if (parsedMessage.Parameters.Length > 2)
            {
                Parameters = parsedMessage.Parameters
                    .Skip(2)
                    .ToArray();
            }
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="target">Mode target.</param>
        /// <param name="modes">Mode flags string.</param>
        public ModeMessage(string target, string modes)
        {
            Target = target;
            Modes = modes;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "MODE", Target, Modes };
    }
}
