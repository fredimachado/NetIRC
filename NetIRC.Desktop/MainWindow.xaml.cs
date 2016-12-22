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

namespace NetIRC.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel mainWindowViewModel;
        private readonly ServerViewModel serverViewModel;

        private string nick = "Fredi_";

        public MainWindow()
        {
            InitializeComponent();

            mainWindowViewModel = new MainWindowViewModel();
            DataContext = mainWindowViewModel;

            serverViewModel = new ServerViewModel();
            mainWindowViewModel.Tabs.Add(serverViewModel);

            tabControl.SelectedIndex = 0;

            serverViewModel.Message("Welcome!");

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            App.Client = new Client(new User(nick, "Fredi"), new TcpClientConnection());

            App.Client.OnRawDataReceived += Client_OnRawDataReceived;
            App.Client.EventHub.Join += EventHub_Join;

            await App.Client.ConnectAsync("irc.rizon.net");
        }

        private void EventHub_Join(Client client, IRCMessageEventArgs<NetIRC.Messages.JoinMessage> e)
        {
            var channel = client.Channels.GetChannel(e.IRCMessage.Channel);

            if (e.IRCMessage.Nick == nick)
            {

                var channelViewModel = new ChannelViewModel(channel);
                channel.MessageReceived += (s, m) => channelViewModel.Message($"{m.User.Nick}: {m.Text}");
                mainWindowViewModel.Tabs.Add(channelViewModel);
                tabControl.SelectedIndex = tabControl.Items.IndexOf(channelViewModel);
            }
        }

        private void Client_OnRawDataReceived(Client client, string rawData)
        {
            serverViewModel.Message(rawData);
        }

        private async void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var textBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                return;
            }

            // if it starts in a slash process the command
            if (textBox.Text.StartsWith("/"))
            {
                await App.Client.SendRaw(textBox.Text.Substring(1));
            }
            else if (tabControl.SelectedItem is ChannelViewModel)
            {
                var channelViewModel = tabControl.SelectedItem as ChannelViewModel;
                channelViewModel.Message($"{nick}: {textBox.Text}");
                await App.Client.SendAsync(new PrivMsgMessage(channelViewModel.Channel.Name, textBox.Text));
            }

            textBox.Text = string.Empty;
        }
    }
}
