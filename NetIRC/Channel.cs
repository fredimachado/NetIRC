using NetIRC.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NetIRC
{
    public class Channel : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ChannelUser> users;
        public ObservableCollection<ChannelUser> Users => users;

        public event EventHandler<ChatMessage> MessageReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public Channel(string name)
        {
            this.name = name;
            users = new ObservableCollection<ChannelUser>();
            users.CollectionChanged += Users_CollectionChanged;
        }

        private void Users_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Users");
        }

        internal void OnMessageReceived(PrivMsgMessage message)
        {
            var user = Users.FirstOrDefault(u => u.Nick == message.From).User;
            MessageReceived?.Invoke(this, new ChatMessage(user, message.Message));
        }

        internal void AddUser(User user, string status)
        {
            Users.Add(new ChannelUser(user, status));
        }

        internal void RemoveUser(string nick)
        {
            var user = Users.FirstOrDefault(u => u.Nick == nick);
            Users.Remove(user);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
