using NetIRC;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace NetIRC.Desktop.ViewModel
{
    public class ChannelViewModel : ITab
    {
        public ObservableCollection<string> Messages { get; private set; }
        public Channel Channel => channel;

        public string Title => channel.Name;

        private readonly Channel channel;

        public ChannelViewModel(Channel channel)
        {
            this.channel = channel;
            Messages = new ObservableCollection<string>();
        }

        public void Message(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() =>
                {
                    Messages.Add(message);
                }));
        }
    }
}
