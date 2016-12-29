using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NetIRC.Desktop.ViewModel
{
    public abstract class TabViewModelBase : ViewModelBase, ITab
    {
        public abstract string Title { get; }

        private string input;
        public string Input
        {
            get { return input; }
            set
            {
                input = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> Messages { get; private set; }

        public ICommand SendCommand { get; }

        public TabViewModelBase()
        {
            Messages = new ObservableCollection<string>();

            SendCommand = new DelegateCommand(async () => await Send());
        }

        protected async virtual Task Send()
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (input.StartsWith("/"))
            {
                await App.Client.SendRaw(input.Substring(1));
            }

            Input = string.Empty;
        }

        public void AddMessage(string message)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action)(() =>
                {
                    Messages.Add(message);
                }));
        }
    }
}
