using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace NetIRC
{
    public class Channel : INotifyPropertyChanged
    {
        public string Name { get; }

        public ObservableCollection<ChannelUser> Users { get; }
        public ObservableCollection<ChatMessage> Messages { get; }

        public event PropertyChangedEventHandler PropertyChanged;

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
            Users.Remove(user);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
