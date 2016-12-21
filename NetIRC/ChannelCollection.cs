using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    public class ChannelCollection : ObservableCollection<Channel>
    {
        public Channel GetChannel(string name)
        {
            var channel = Items.FirstOrDefault(c => c.Name == name);

            if (channel == null)
            {
                channel = new Channel(name);
                Add(channel);
            }

            return channel;
        }
    }
}
