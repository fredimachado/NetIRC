using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace NetIRC.Desktop.ViewModel
{
    public class ServerViewModel : ITab
    {
        public string Title => "Server";

        public ObservableCollection<string> Messages { get; private set; }

        public ServerViewModel()
        {
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
