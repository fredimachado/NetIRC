using NetIRC.Connection;
using NetIRC.Ctcp;
using NetIRC.Messages;
using System;
using System.Linq;
using System.Reflection;
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
        /// Enables a custom dispatcher to be used if necessary. For example, WPF Dispatcher, to make sure collections are manipulated in the UI thread
        /// </summary>
        internal static Action<Action> DispatcherInvoker;

        /// <summary>
        /// Represents the user used to connect to the server
        /// </summary>
        public User User { get; }

        /// <summary>
        /// An observable collection representing server messages
        /// </summary>
        public ServerMessageCollection ServerMessages { get; } = new ServerMessageCollection();

        /// <summary>
        /// An observable collection representing the channels we joined
        /// </summary>
        public ChannelCollection Channels { get; } = new ChannelCollection();

        /// <summary>
        /// An observable collection representing all queries (private chat)
        /// </summary>
        public QueryCollection Queries { get; } = new QueryCollection();

        /// <summary>
        /// An observable collection representing all peers (users) the client knows about
        /// It can be channel users, or query users (private chat)
        /// </summary>
        public UserCollection Peers { get; } = new UserCollection();

        /// <summary>
        /// Indicates that we received raw data from the server and gives you access to the data
        /// </summary>
        public event IRCRawDataHandler RawDataReceived;

        /// <summary>
        /// Indicates that we have parsed the message and gives you a strong typed representation of it
        /// You get the prefix, command, parameters and some other goodies
        /// </summary>
        public event ParsedIRCMessageHandler IRCMessageParsed;

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
        /// Indicates that we received a CTCP message (Client-To-Client-Protocol)
        /// </summary>
        public event CtcpHandler CtcpReceived;
        internal void OnCtcpReceived(CtcpEventArgs ctcp)
        {
            CtcpReceived?.Invoke(this, ctcp);

            CtcpCommands.HandleCtcp(this, ctcp);
        }

        /// <summary>
        /// Initializes a new instance of the IRC client with a User and an IConnection implementation
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        /// <param name="connection">IConnection implementation</param>
        public Client(User user, IConnection connection)
        {
            User = user;
            this.connection = connection;

            DispatcherInvoker = a => a.Invoke();

            messageHandlerContainer = new MessageHandlerContainer(this);

            this.connection.DataReceived += Connection_DataReceived;
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

        private async void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var rawData = e.Data;

            RawDataReceived?.Invoke(this, e.Data);

            var parsedIRCMessage = new ParsedIRCMessage(rawData);

            await HandleServerMessages(parsedIRCMessage);

            IRCMessageParsed?.Invoke(this, parsedIRCMessage);

            await messageHandlerContainer.HandleAsync(parsedIRCMessage)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Connects to the specified IRC server using the specified port number
        /// </summary>
        /// <param name="host">IRC server address</param>
        /// <param name="port">Port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync()
        {
            await connection.ConnectAsync()
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

        /// <summary>
        /// Adds a custom message handler of the type specified in TCustomMessageHandler
        /// </summary>
        /// <typeparam name="TCustomMessageHandler">The type of the custom message handler to add.</typeparam>
        public void RegisterCustomMessageHandler<TCustomMessageHandler>()
            where TCustomMessageHandler : ICustomHandler
        {
            messageHandlerContainer.RegisterCustomMessageHandler(typeof(TCustomMessageHandler));
        }

        /// <summary>
        /// Adds all custom message handlers present in a specific assembly
        /// </summary>
        /// <param name="assembly">The assembly containing custom message handlers to add.</param>
        public void RegisterCustomMessageHandlers(Assembly assembly)
        {
            messageHandlerContainer.RegisterCustomMessageHandlers(assembly);
        }

        /// <summary>
        /// Sets the internal dispatcher invoker so collections get manipulated in a specific thread
        /// For WPF you can pass Application.Dispatcher.Invoke
        /// </summary>
        /// <param name="dispatcherInvoke"></param>
        public void SetDispatcherInvoker(Action<Action> dispatcherInvoke)
        {
            _ = dispatcherInvoke ?? throw new ArgumentNullException(nameof(dispatcherInvoke));

            DispatcherInvoker = dispatcherInvoke;
        }

        private Task HandleServerMessages(ParsedIRCMessage parsedIRCMessage)
        {
            if (parsedIRCMessage.IsNumeric)
            {
                return HandleNumericReply(parsedIRCMessage);
            }

            return Task.CompletedTask;
        }

        private Task HandleNumericReply(ParsedIRCMessage parsedIRCMessage)
        {
            string text = string.Empty;
            switch (parsedIRCMessage.NumericReply)
            {
                case IRCNumericReply.RPL_MYINFO:
                case IRCNumericReply.RPL_ISUPPORT:
                    text = string.Join(" ", parsedIRCMessage.Parameters.Skip(1));
                    break;
                case IRCNumericReply.RPL_LUSEROP:
                case IRCNumericReply.RPL_LUSERUNKNOWN:
                case IRCNumericReply.RPL_LUSERCHANNELS:
                    text = $"{parsedIRCMessage.Parameters[1]} {parsedIRCMessage.Trailing}";
                    break;
                case IRCNumericReply.RPL_NAMREPLY:
                case IRCNumericReply.RPL_ENDOFNAMES:
                    return Task.CompletedTask;
                default:
                    text = parsedIRCMessage.Trailing;
                    break;
            }

            if (!string.IsNullOrEmpty(text))
            {
                DispatcherInvoker.Invoke(() => ServerMessages.Add(new ServerMessage(text)));
            }

            return Task.CompletedTask;
        }
    }
}
