using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Threading.Tasks;

namespace NetIRC.ConsoleCli
{
    public class Program
    {
        public const string MyCommander = "Fredi_"; // Who can control me
        public const string CommandPrefix = "EXECUTE ";

        private const string nickName = "NetIRCConsoleClient";
        private const string channel = "#NetIRCChannel";

        // Set this to true if you want all raw messages received from the server to be written to the console
        private const bool verbose = false;

        private static Client client;

        static void Main(string[] args)
        {
            // User connecting to the IRC server
            var user = new User(nickName, "NetIRC");

            var tcpConnection = new TcpClientConnection("irc.rizon.net", 6667);

            // Create IRC client instance, wrapped in a using statement so it gets properly disposed (IDisposable pattern)
            using (client = new Client(user, tcpConnection))
            {
                // Subscribe to IRC client events
                client.RawDataReceived += Client_RawDataReceived;
                client.IRCMessageParsed += Client_IRCMessageParsed;
                client.RegistrationCompleted += EventHub_RegistrationCompleted;

                // Queries is an ObservableCollection, so we can subscribe to the CollectionChanged event
                // A new Query will be automatically created when initiating a private conversation with someone
                client.Queries.CollectionChanged += Queries_CollectionChanged;

                // Channels is also an ObservableCollection, so we can subscribe to the CollectionChanged event
                // A new Channel will be automatically created when joining a channel
                client.Channels.CollectionChanged += Channels_CollectionChanged;

                // Handy method to register all custom message handlers in an assembly
                client.RegisterCustomMessageHandlers(typeof(Program).Assembly);

                // Custom message handlers can also be manually registered
                //client.RegisterCustomMessageHandler<PrivMsgHandler>();

                // Connect to the server and let the magic happen in the background
                Task.Run(() => client.ConnectAsync());

                // Wait for a key press before exiting the console application
                Console.Read();
            }
        }

        // This event handler will be called everytime a raw message is received from the server (interesting to inspect the IRC protocol)
        private static void Client_RawDataReceived(Client client, string rawData)
        {
            if (verbose)
            {
#pragma warning disable CS0162 // Suppress warning: Unreachable code detected
                WriteLine(rawData);
#pragma warning restore CS0162 // Suppress warning: Unreachable code detected
            }
        }

        // This event handler will be called after the IRC client has parsed a raw message (nicer to work with when compared to the raw message string)
        private static async void Client_IRCMessageParsed(Client client, ParsedIRCMessage ircMessage)
        {
            // When receiving a topic message
            // NOTE: Just an example, because having a custom message handler deriving from CustomMessageHandler<TopicMessage> would be better
            // as TopicMessage will have the message parsed even further
            if (ircMessage.IRCCommand == IRCCommand.TOPIC)
            {
                var channel = ircMessage.Parameters[0];
                var topic = ircMessage.Trailing;
                WriteLine($"* {channel}'s TOPIC: {topic}", ConsoleColor.Cyan);
            }

            // If we joined a channel, or someone joined a channel we're in
            // NOTE: Just as an example. We can use a custom message handler deriving from CustomMessageHandler<JoinMessage>
            if (ircMessage.IRCCommand == IRCCommand.JOIN)
            {
                // At this point we can create an instance of JoinMessage using the ParsedIRCMessage (Since we know for sure this is a JOIN message)
                // This could have been done in the TOPIC message above with: var topicMessage = new TopicMessage(ircMessage);
                // Having a more specific message object saves us from having to get information from Parameters and Trailing properties (as seen above with TOPIC)
                var joinMessage = new JoinMessage(ircMessage);

                // If we joined the #NetIRCChannel channel, change its topic
                if (joinMessage.Nick == nickName && joinMessage.Channel == channel)
                {
                    await client.SendAsync(new TopicMessage(channel, "NetIRC is nice!!"));
                }
            }

        }

        // This event handler will be called when the user registration has been completed (usefull to know when we can start sending messages to the server)
        private static async void EventHub_RegistrationCompleted(object sender, EventArgs e)
        {
            WriteLine("Ready to Roll!", ConsoleColor.Yellow);

            await client.SendAsync(new JoinMessage(channel));
        }

        // This event handler will be called when a new Query is created (after initiating a new private conversation)
        private static void Queries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Explicitly using the Query type in the foreach
            foreach (Query item in e.NewItems)
            {
                // Query.Messages is an ObservableCollection
                // So let's subscribe to it in order to do something with private messages received
                item.Messages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private static void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Explicitly using the QueryMessage type in the foreach
            foreach (QueryMessage item in e.NewItems)
            {
                // Print private message
                WriteLine($"<{item.User.Nick}> {item.Text}", ConsoleColor.Green);
            }
        }

        private static void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Explicitly using the Channel type in the foreach
            foreach (Channel item in e.NewItems)
            {
                item.Messages.CollectionChanged += ChannelMessages_CollectionChanged;
            }
        }

        private static void ChannelMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Explicitly using the ChannelMessage type in the foreach
            foreach (ChannelMessage item in e.NewItems)
            {
                // Print channel message
                WriteLine($"[{item.Channel.Name}] <{item.User.Nick}> {item.Text}", ConsoleColor.Yellow);
            }
        }

        // Just a simple Console.WriteLine wrapper to allow us to change font color
        public static void WriteLine(string value, ConsoleColor color = ConsoleColor.White)
        {
            var previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = previousColor;
        }
    }
}
