using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a PONG response command.
    /// </summary>
    public class PongMessage : IRCMessage, IClientMessage
    {
        /// <summary>
        /// Gets the PONG target value.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="PongMessage"/>.
        /// </summary>
        /// <param name="target">PING token to respond to.</param>
        public PongMessage(string target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "PONG", Target };
    }
}
