﻿using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// An observable collection that represents all channels we joined
    /// </summary>
    public class ChannelCollection : ObservableCollection<Channel>
    {
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
