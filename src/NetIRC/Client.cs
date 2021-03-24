using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetIRC
{
    /// <summary>
    /// The NetIRC IRC client
    /// </summary>
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        private readonly string password;

        private readonly MessageHandlerContainer messageHandlerContainer;

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
        public event IRCRawDataHandler OnRawDataReceived;

        /// <summary>
        /// Indicates that we have parsed the message and gives you a strong typed representation of it
        /// You get the prefix, command, parameters and some other goodies
        /// </summary>
        public event ParsedIRCMessageHandler OnIRCMessageParsed;

        /// <summary>
        /// Indicates that we are properly registered on the server
        /// It happens when the server sends a 001 (Welcome) reply to a user upon successful registration
        /// </summary>
        public event EventHandler RegistrationCompleted;
        internal void OnRegistrationCompleted()
        {
            RegistrationCompleted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the IRC client with a User and a default IConnection implementation (TcpClientConnection)
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        public Client(User user)
            : this(user, new TcpClientConnection())
        {}

        /// <summary>
        /// Initializes a new instance of the IRC client with a User, password and a default IConnection implementation (TcpClientConnection)
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        /// <param name="password">Password to use when connecting to the server</param>
        public Client(User user, string password)
            : this(user, password, new TcpClientConnection())
        { }

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

            messageHandlerContainer = new MessageHandlerContainer(this);
        }

        /// <summary>
        /// Initializes a new instance of the IRC client with a User, password and an IConnection implementation
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        /// <param name="password">Password to use when connecting to the server</param>
        /// <param name="connection">IConnection implementation</param>
        public Client(User user, string password, IConnection connection)
            : this(user, connection)
        {
            this.password = password;
        }

        public void RegisterCustomMessageHandler(Type type)
        {
            messageHandlerContainer.RegisterCustomMessageHandler(type);
        }

        private async void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            var parsedIRCMessage = new ParsedIRCMessage(rawData);

            OnIRCMessageParsed?.Invoke(this, parsedIRCMessage);

            await messageHandlerContainer.HandleAsync(parsedIRCMessage)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Connects to the specified IRC server using the specified port number
        /// </summary>
        /// <param name="host">IRC server address</param>
        /// <param name="port">Port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync(string host, int port = 6667)
        {
            await connection.ConnectAsync(host, port)
                .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(password))
            {
                await SendAsync(new PassMessage(password))
                    .ConfigureAwait(false);
            }
            await SendAsync(new NickMessage(User.Nick))
                    .ConfigureAwait(false);
            await SendAsync(new UserMessage(User.Nick, User.RealName))
                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Allows you to send raw data the the IRC server
        /// </summary>
        /// <param name="rawData">The raw data to be sent</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task SendRaw(string rawData)
        {
            return connection.SendAsync(rawData);
        }

        /// <summary>
        /// Allows you to send a strong typed client message to the IRC server
        /// </summary>
        /// <param name="message">An implementation of IClientMessage. Check NetIRC.Messages namespace</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public Task SendAsync(IClientMessage message)
        {
            return connection.SendAsync(message.ToString());
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
