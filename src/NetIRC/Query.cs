using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NetIRC
{
    public class Query : INotifyPropertyChanged
    {
        public User User { get; }
        public string Nick => User.Nick;

        public ObservableCollection<ChatMessage> Messages { get; }

        public Query(User user)
        {
            User = user;

            Messages = new ObservableCollection<ChatMessage>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
