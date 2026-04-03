using System;

namespace NetIRC
{
    /// <summary>
    /// Represents a query message (private message)
    /// </summary>
    public class QueryMessage : EventArgs
    {
        /// <summary>
        /// Gets the user who sent the message.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets the private message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the local timestamp when the message was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="QueryMessage"/>.
        /// </summary>
        /// <param name="user">Message author.</param>
        /// <param name="text">Message content.</param>
        public QueryMessage(User user, string text)
        {
            User = user;
            Text = text;
            Timestamp = DateTime.Now;
        }
    }
}
