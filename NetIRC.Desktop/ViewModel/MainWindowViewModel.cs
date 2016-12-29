using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetIRC.Desktop.ViewModel
{
    public class MainWindowViewModel
    {
        public ObservableCollection<ITab> Tabs { get; }

        public ICommand Connect { get; }

        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<ITab>();

            Connect = new DelegateCommand(async () => await ConnectCommand());
        }

        private async Task ConnectCommand()
        {
            await App.Client.ConnectAsync("irc.rizon.net");
        }
    }
}
