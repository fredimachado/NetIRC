using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a NOTICE message from server or client.
    /// </summary>
    public class NoticeMessage : IRCMessage, IServerMessage, IClientMessage
    {
        /// <summary>
        /// Gets the sender nickname.
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Gets the target nickname or channel.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// Gets the notice text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public NoticeMessage(ParsedIRCMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Target = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="target">Target nickname or channel.</param>
        /// <param name="text">Notice text.</param>
        public NoticeMessage(string target, string text)
        {
            Target = target;
            Message = text;
        }

        /// <summary>
        /// Gets whether the target is a channel.
        /// </summary>
        public bool IsChannelMessage => Target[0] == '#';

        /// <summary>
        /// Gets command tokens for wire serialization.
        /// </summary>
        public IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };
    }
}
