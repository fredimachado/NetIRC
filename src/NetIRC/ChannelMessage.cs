﻿using System;

namespace NetIRC
{
    /// <summary>
    /// Represents a channel message
    /// </summary>
    public class ChannelMessage : EventArgs
    {
        public User User { get; }
        public Channel Channel { get; }
        public string Text { get; }
        public DateTime Timestamp { get; }

        public ChannelMessage(User user, Channel channel, string text)
        {
            User = user;
            Channel = channel;
            Text = text;
            Timestamp = DateTime.Now;
        }
    }
}
