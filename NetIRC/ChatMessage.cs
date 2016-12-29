using System;

namespace NetIRC
{
    public class ChatMessage : EventArgs
    {
        public User User { get; }
        public string Text { get; }
        public DateTime Date { get; }

        public string Nick => User.Nick;

        public ChatMessage(User user, string text)
        {
            User = user;
            Text = text;
            Date = DateTime.Now;
        }
    }
}
