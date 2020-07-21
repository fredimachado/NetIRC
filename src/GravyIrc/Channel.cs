﻿using System.Collections.ObjectModel;
using System.Linq;

namespace GravyIrc
{
    /// <summary>
    /// Represents an IRC channel with its users and messages.
    /// </summary>
    public class Channel
    {
        public string Name { get; }

        public ObservableCollection<ChannelUser> Users { get; }
        public ObservableCollection<ChatMessage> Messages { get; }

        public Channel(string name)
        {
            Name = name;
            Users = new ObservableCollection<ChannelUser>();
            Messages = new ObservableCollection<ChatMessage>();
        }

        internal void AddUser(User user, string status)
        {
            Users.Add(new ChannelUser(user, status));
        }

        internal void RemoveUser(string nick)
        {
            var user = Users.FirstOrDefault(u => u.Nick == nick);
            if (user != null)
                Users.Remove(user);
        }

        public ChannelUser GetUser(string nick) => Users.FirstOrDefault(u => u.Nick.ToLower() == nick.ToLower());
    }
}
