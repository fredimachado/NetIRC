using GravyIrc.Connection;
using GravyIrc.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GravyIrc
{
    /// <summary>
    /// The NetIRC IRC client
    /// </summary>
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        private readonly string password;

        /// <summary>
        /// Represents the user used to connect to the server
        /// </summary>
        public User User { get; }

        /// <summary>
        /// An observable collection representing the channels we joined
        /// </summary>
        public ChannelCollection Channels { get; }

        /// <summary>
        /// An observable collection representing all queries (private chat)
        /// </summary>
        public QueryCollection Queries { get; }

        /// <summary>
        /// An observable collection representing all peers (users) the client knows about
        /// It can be channel users, or query users (private chat)
        /// </summary>
        public UserCollection Peers { get; }

        /// <summary>
        /// Indicates that we received raw data from the server and gives you access to the data
        /// </summary>
        public event IrcRawDataHandler OnRawDataReceived;

        /// <summary>
        /// Indicates that we have parsed the message and gives you a strong typed representation of it
        /// You get the prefix, command, parameters and some other goodies
        /// </summary>
        public event ParsedIrcMessageHandler OnIrcMessageParsed;

        /// <summary>
        /// Provides you a way to handle various IRC events like OnPing and OnPrivMsg
        /// </summary>
        public EventHub EventHub { get; }

        /// <summary>
        /// Initializes a new instance of the IRC client with a User and an IConnection implementation
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        /// <param name="connection">IConnection implementation</param>
        public Client(User user, IConnection connection)
        {
            User = user;

            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;

            Channels = new ChannelCollection();
            Queries = new QueryCollection();
            Peers = new UserCollection();

            EventHub = new EventHub(this);
            InitializeDefaultEventHubEvents();
        }

        public Client(User user, string password, IConnection connection)
            : this(user, connection)
        {
            this.password = password;
        }

        private void InitializeDefaultEventHubEvents()
        {
            EventHub.Ping += EventHub_Ping;
            EventHub.Join += EventHub_Join;
            EventHub.Part += EventHub_Part;
            EventHub.Quit += EventHub_Quit;
            EventHub.Kick += EventHub_Kick;
            EventHub.PrivMsg += EventHub_PrivMsg;
            EventHub.RplNamReply += EventHub_RplNamReply;
            EventHub.Nick += EventHub_Nick;
        }

        private void EventHub_Nick(Client client, IrcMessageEventArgs<NickMessage> e)
        {
            var user = Peers.GetUser(e.IrcMessage.OldNick);
            user.Nick = e.IrcMessage.NewNick;
        }

        private void EventHub_PrivMsg(Client client, IrcMessageEventArgs<PrivMsgMessage> e)
        {
            var user = Peers.GetUser(e.IrcMessage.From);
            var message = new ChatMessage(user, e.IrcMessage.Message);

            if (e.IrcMessage.IsChannelMessage)
            {
                var channel = Channels.GetChannel(e.IrcMessage.To);
                channel.Messages.Add(message);
            }
            else
            {
                var query = Queries.GetQuery(user);
                query.Messages.Add(message);
            }
        }

        private void EventHub_RplNamReply(Client client, IrcMessageEventArgs<RplNamReplyMessage> e)
        {
            var channel = Channels.GetChannel(e.IrcMessage.Channel);
            foreach (var nick in e.IrcMessage.Nicks)
            {
                var user = Peers.GetUser(nick.Key);
                if (!channel.Users.Any(u => u.User.Nick == nick.Key))
                {
                    channel.AddUser(user, nick.Value);
                }
            }
        }

        private void EventHub_Quit(Client client, IrcMessageEventArgs<QuitMessage> e)
        {
            foreach (var channel in Channels)
            {
                channel.RemoveUser(e.IrcMessage.Nick);
            }
        }

        private void EventHub_Kick(Client client, IrcMessageEventArgs<KickMessage> e)
        {
            var channel = Channels.FirstOrDefault(c => c.Name == e.IrcMessage.Channel);
            if (channel != null)
            {
                if (e.IrcMessage.Nick == User.Nick)
                {
                    Channels.Remove(channel);
                }
                else
                {
                    channel.RemoveUser(e.IrcMessage.Nick);
                }
            }
        }

        private void EventHub_Part(Client client, IrcMessageEventArgs<PartMessage> e)
        {
            var channel = Channels.GetChannel(e.IrcMessage.Channel);
            channel.RemoveUser(e.IrcMessage.Nick);
        }

        private void EventHub_Join(Client client, IrcMessageEventArgs<JoinMessage> e)
        {
            var channel = Channels.GetChannel(e.IrcMessage.Channel);
            if (e.IrcMessage.Nick != User.Nick)
            {
                var user = Peers.GetUser(e.IrcMessage.Nick);
                channel.AddUser(user, string.Empty);
            }
        }

        private async void EventHub_Ping(object sender, IrcMessageEventArgs<PingMessage> e)
        {
            await SendAsync(new PongMessage(e.IrcMessage.Target));
        }

        private void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            var parsedIRCMessage = new ParsedIrcMessage(rawData);

            OnIrcMessageParsed?.Invoke(this, parsedIRCMessage);

            var serverMessage = IrcMessage.Create(parsedIRCMessage);

            serverMessage?.TriggerEvent(EventHub);
        }

        /// <summary>
        /// Connects to the specified IRC server using the specified port number
        /// </summary>
        /// <param name="host">IRC server address</param>
        /// <param name="port">Port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync(string host, int port = 6667)
        {
            await connection.ConnectAsync(host, port);

            if (!string.IsNullOrWhiteSpace(password))
            {
                await SendAsync(new PassMessage(password));
            }
            await SendAsync(new NickMessage(User.Nick));
            await SendAsync(new UserMessage(User.Nick, User.RealName));
        }

        /// <summary>
        /// Allows you to send raw data the the IRC server
        /// </summary>
        /// <param name="rawData">The raw data to be sent</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendRaw(string rawData)
        {
            await connection.SendAsync(rawData);
        }

        /// <summary>
        /// Allows you to send a strong typed client message to the IRC server
        /// </summary>
        /// <param name="message">An implementation of IClientMessage. Check NetIRC.Messages namespace</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendAsync(IClientMessage message)
        {
            await connection.SendAsync(message.ToString());
        }

        /// <summary>
        /// Disposes the connection
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
