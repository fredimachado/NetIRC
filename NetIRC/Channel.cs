using System.Collections.Generic;
using System.ComponentModel;
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

        public ICollection<ChannelUser> Users { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Channel(string name)
        {
            this.name = name;
            Users = new List<ChannelUser>();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
