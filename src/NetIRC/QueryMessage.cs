using System;

namespace NetIRC
{
    /// <summary>
    /// Represents a query message (private message)
    /// </summary>
    public class QueryMessage : EventArgs
    {
        public User User { get; }
        public string Text { get; }
        public DateTime Date { get; }

        public QueryMessage(User user, string text)
        {
            User = user;
            Text = text;
            Date = DateTime.Now;
        }
    }
}
