using NetIRC;
using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NetIRC.Desktop.ViewModel;
using MahApps.Metro.Controls;

namespace NetIRC.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ServerViewModel serverViewModel;

        public MainWindow()
        {
            InitializeComponent();

            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;

            serverViewModel = new ServerViewModel();
            mainWindowViewModel.Tabs.Add(serverViewModel);

            tabControl.SelectedIndex = 0;

            serverViewModel.AddMessage("Welcome!");

            App.Nick = "Fredi_";

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.Client = new Client(new User(App.Nick, "Fredi"), new TcpClientConnection());

            App.Client.OnRawDataReceived += Client_OnRawDataReceived;

            App.Client.Channels.CollectionChanged += Channels_CollectionChanged;
            App.Client.Queries.CollectionChanged += Queries_CollectionChanged;
        }

        private void Queries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (Query query in e.NewItems)
                {
                    var queryVM = new QueryViewModel(query);
                    mainWindowViewModel.Tabs.Add(queryVM);
                }
            }
        }

        private void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (Channel channel in e.NewItems)
                {
                    var channelVM = new ChannelViewModel(channel);
                    mainWindowViewModel.Tabs.Add(channelVM);
                }
            }
        }

        private void Client_OnRawDataReceived(Client client, string rawData)
        {
            serverViewModel.AddMessage(rawData);
        }
    }
}
