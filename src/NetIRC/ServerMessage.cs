using System;

namespace NetIRC
{
    /// <summary>
    /// Represents an informational message received from the server.
    /// </summary>
    public class ServerMessage
    {
        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the local timestamp when the message was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ServerMessage"/>.
        /// </summary>
        /// <param name="text">Message text to display.</param>
        public ServerMessage(string text)
        {
            Text = text;
            Timestamp = DateTime.Now;
        }
    }
}
