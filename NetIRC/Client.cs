using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetIRC
{
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        public User User { get;}
        public ChannelCollection Channels { get; }
        public UserCollection Users { get; }

        public event IRCRawDataHandler OnRawDataReceived;
        public event ParsedIRCMessageHandler OnIRCMessageParsed;

        public EventHub EventHub { get; }

        public Client(User user, IConnection connection)
        {
            User = user;

            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;

            Channels = new ChannelCollection();
            Users = new UserCollection();

            EventHub = new EventHub(this);
            InitializeDefaultEventHubEvents();
        }

        private void InitializeDefaultEventHubEvents()
        {
            EventHub.Ping += EventHub_Ping;
            EventHub.Join += EventHub_Join;
            EventHub.Part += EventHub_Part;
            EventHub.Quit += EventHub_Quit;
            EventHub.PrivMsg += EventHub_PrivMsg;
            EventHub.RplNamReply += EventHub_RplNamReply;
        }

        private void EventHub_PrivMsg(Client client, IRCMessageEventArgs<PrivMsgMessage> e)
        {
            if (e.IRCMessage.To[0] == '#')
            {
                var channel = Channels.GetChannel(e.IRCMessage.To);
                channel.OnMessageReceived(e.IRCMessage);
            }
        }

        private void EventHub_RplNamReply(Client client, IRCMessageEventArgs<RplNamReplyMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            foreach (var nick in e.IRCMessage.Nicks)
            {
                var user = Users.GetUser(nick.Key);
                if (!channel.Users.Any(u => u.User.Nick == nick.Key))
                {
                    channel.AddUser(user, nick.Value);
                }
            }
        }

        private void EventHub_Quit(Client client, IRCMessageEventArgs<QuitMessage> e)
        {
            foreach (var channel in Channels)
            {
                var user = channel.Users.FirstOrDefault(u => u.Nick == e.IRCMessage.Nick);
                if (user != null)
                {
                    channel.Users.Remove(user);
                }
            }
        }

        private void EventHub_Part(Client client, IRCMessageEventArgs<PartMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            channel.RemoveUser(e.IRCMessage.Nick);
        }

        private void EventHub_Join(Client client, IRCMessageEventArgs<JoinMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            if (e.IRCMessage.Nick != User.Nick)
            {
                var user = Users.GetUser(e.IRCMessage.Nick);
                channel.AddUser(user, string.Empty);
            }
        }

        private async void EventHub_Ping(object sender, IRCMessageEventArgs<PingMessage> e)
        {
            await SendAsync(new PongMessage(e.IRCMessage.Target));
        }

        private void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            var parsedIRCMessage = new ParsedIRCMessage(rawData);

            OnIRCMessageParsed?.Invoke(this, parsedIRCMessage);

            var serverMessage = IRCMessage.Create(parsedIRCMessage);

            serverMessage?.TriggerEvent(EventHub);
        }

        public async Task ConnectAsync(string host, int port = 6667)
        {
            await connection.ConnectAsync(host, port);

            await SendAsync(new NickMessage(User.Nick));
            await SendAsync(new UserMessage(User.Nick, User.RealName));
        }

        public async Task SendRaw(string rawData)
        {
            await connection.SendAsync(rawData);
        }

        public async Task SendAsync(IClientMessage message)
        {
            await connection.SendAsync(message.ToString());
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
