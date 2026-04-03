using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a PASS command used for server authentication.
    /// </summary>
    public class PassMessage : IRCMessage, IClientMessage
    {
        private readonly string password;

        /// <summary>
        /// Initializes a new instance of <see cref="PassMessage"/>.
        /// </summary>
        /// <param name="password">Connection password.</param>
        public PassMessage(string password)
        {
            this.password = password;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "PASS", password };
    }
}
