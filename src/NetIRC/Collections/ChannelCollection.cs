using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// An observable collection that represents all channels we joined
    /// </summary>
    public class ChannelCollection : ObservableCollection<Channel>
    {
        /// <summary>
        /// Gets an existing channel by name, or creates one if it does not exist.
        /// </summary>
        /// <param name="name">Channel name.</param>
        /// <returns>The existing or newly created <see cref="Channel"/>.</returns>
        public Channel GetChannel(string name)
        {
            var channel = Items.FirstOrDefault(c => c.Name == name);

            if (channel is null)
            {
                channel = new Channel(name);
                Client.DispatcherInvoker.Invoke(() => Add(channel));
            }

            return channel;
        }
    }
}
