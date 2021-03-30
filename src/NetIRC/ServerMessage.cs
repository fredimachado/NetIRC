using System;

namespace NetIRC
{
    public class ServerMessage
    {
        public string Text { get; }
        public DateTime Date { get; }

        public ServerMessage(string text)
        {
            Text = text;
            Date = DateTime.Now;
        }
    }
}
