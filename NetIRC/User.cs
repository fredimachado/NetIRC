using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetIRC
{
    public class User : INotifyPropertyChanged
    {
        public User(string nick)
        {
            Nick = nick;
        }

        public User(string nick, string realName)
        {
            Nick = nick;
            RealName = realName;
        }

        public string Nick { get; set; }
        public string RealName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
