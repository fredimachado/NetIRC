using NetIRC.Ctcp;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a CTCP reply serialized as an IRC NOTICE command.
    /// </summary>
    public class CtcpReplyMessage : IClientMessage
    {
        /// <summary>
        /// Gets the target nickname.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets the CTCP-wrapped notice payload.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="CtcpReplyMessage"/>.
        /// </summary>
        /// <param name="target">Reply target nickname.</param>
        /// <param name="text">CTCP payload text.</param>
        public CtcpReplyMessage(string target, string text)
        {
            Target = target;
            Message = $":{CtcpCommands.CtcpDelimiter}{text}{CtcpCommands.CtcpDelimiter}";
        }

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };

        /// <summary>
        /// Converts this reply to an IRC command string.
        /// </summary>
        /// <returns>Serialized NOTICE command.</returns>
        public override string ToString() => string.Join(" ", Tokens);
    }
}
