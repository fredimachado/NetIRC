using System;

namespace NetIRC
{
    /// <summary>
    /// Represents a channel message
    /// </summary>
    public class ChannelMessage : EventArgs
    {
        /// <summary>
        /// Gets the user who sent the message.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets the channel where the message was sent.
        /// </summary>
        public Channel Channel { get; }

        /// <summary>
        /// Gets the channel message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the local timestamp when the message was created.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ChannelMessage"/>.
        /// </summary>
        /// <param name="user">Message author.</param>
        /// <param name="channel">Target channel.</param>
        /// <param name="text">Message content.</param>
        public ChannelMessage(User user, Channel channel, string text)
        {
            User = user;
            Channel = channel;
            Text = text;
            Timestamp = DateTime.Now;
        }
    }
}
