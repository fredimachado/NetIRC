using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// Represents an IRC channel with its users and messages.
    /// </summary>
    public class Channel
    {
        public string Name { get; }
        public string Topic { get; private set; }

        public ObservableCollection<ChannelUser> Users { get; }
        public ObservableCollection<ChannelMessage> Messages { get; }

        public Channel(string name)
        {
            Name = name;
            Users = new ObservableCollection<ChannelUser>();
            Messages = new ObservableCollection<ChannelMessage>();
        }

        internal void AddUser(User user)
        {
            AddUser(user, string.Empty);
        }

        internal void AddUser(User user, string status)
        {
            Client.DispatcherInvoker.Invoke(() => Users.Add(new ChannelUser(user, status)));
        }

        internal void RemoveUser(string nick)
        {
            var user = GetUser(nick);
            if (user != null)
            {
                Client.DispatcherInvoker.Invoke(() => Users.Remove(user));
            }
        }

        internal void SetTopic(string topic)
        {
            Topic = topic;
        }

        public ChannelUser GetUser(string nick)
            => Users.FirstOrDefault(u => string.Equals(u.Nick, nick, StringComparison.InvariantCultureIgnoreCase));
    }
}
