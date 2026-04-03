using System;

namespace NetIRC.Messages
{
    /// <summary>
    /// Associates a message type or handler with an IRC command token.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Gets the IRC command mapped to the attributed type.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CommandAttribute"/>.
        /// </summary>
        /// <param name="command">IRC command token or numeric reply code.</param>
        public CommandAttribute(string command)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            Command = command.ToUpper();
        }
    }
}
