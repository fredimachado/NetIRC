using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a USER command sent during registration.
    /// </summary>
    public class UserMessage : IRCMessage, IClientMessage
    {
        /// <summary>
        /// Gets the IRC username.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets the real name portion of the command.
        /// </summary>
        public string RealName { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="UserMessage"/>.
        /// </summary>
        /// <param name="userName">IRC username.</param>
        /// <param name="realName">Real name value.</param>
        public UserMessage(string userName, string realName)
        {
            UserName = userName;
            RealName = realName;
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "USER", UserName, "0", "-", RealName };
    }
}
