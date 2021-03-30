using System;

namespace NetIRC
{
    public class ServerMessage
    {
        public string Text { get; }
        public DateTime Timestamp { get; }

        public ServerMessage(string text)
        {
            Text = text;
            Timestamp = DateTime.Now;
        }
    }
}
