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
        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the current channel topic.
        /// </summary>
        public string Topic { get; private set; }

        /// <summary>
        /// Gets the users currently known in the channel.
        /// </summary>
        public ObservableCollection<ChannelUser> Users { get; }

        /// <summary>
        /// Gets channel messages.
        /// </summary>
        public ObservableCollection<ChannelMessage> Messages { get; }

        internal static char[] UserStatuses = new[] { '~', '&', '@', '%', '+' };

        /// <summary>
        /// Initializes a new instance of <see cref="Channel"/>.
        /// </summary>
        /// <param name="name">Channel name.</param>
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

        /// <summary>
        /// Gets a channel user by nickname.
        /// </summary>
        /// <param name="nick">Nickname to search for.</param>
        /// <returns>The matching <see cref="ChannelUser"/> or <see langword="null"/>.</returns>
        public ChannelUser GetUser(string nick)
            => Users.FirstOrDefault(u => string.Equals(u.Nick, nick, StringComparison.InvariantCultureIgnoreCase));
    }
}
