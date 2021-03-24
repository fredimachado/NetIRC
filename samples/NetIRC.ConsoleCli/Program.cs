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

            // Create IRC client instance, wrapped in a using statement so it gets properly disposed (IDisposable pattern)
            using (client = new Client(user))
            {
                // Subscribe to IRC client events
                client.OnRawDataReceived += Client_OnRawDataReceived;
                client.OnIRCMessageParsed += Client_OnIRCMessageParsed;
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
                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667));

                // Wait for a key press before exiting the console application
                Console.Read();
            }
        }

        // This event handler will be called everytime a raw message is received from the server (interesting to inspect the IRC protocol)
        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            if (verbose)
            {
                WriteLine(rawData);
            }
        }

        // This event handler will be called after the IRC client has parsed a raw message (nicer to work with when compared to the raw message string)
        private static async void Client_OnIRCMessageParsed(Client client, ParsedIRCMessage ircMessage)
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

    [Command("PRIVMSG")] // Optional since the message name is present just before the suffix (Handler) in the class name
    public class PrivMsgHandler : CustomMessageHandler<PrivMsgMessage>
    {
        public override async Task HandleAsync(PrivMsgMessage serverMessage, Client client)
        {
            // If message starts from a specific text ("EXECUTE " in this case)
            // And message came from a specific user with Nick equal to "Fredi_"
            if (serverMessage.Message.StartsWith(Program.CommandPrefix, StringComparison.InvariantCultureIgnoreCase)
                && serverMessage.From.Equals(Program.MyCommander, StringComparison.InvariantCultureIgnoreCase))
            {
                // Creates new string with prefix ("EXECUTE ") removed from message
                var rawMessage = serverMessage.Message[Program.CommandPrefix.Length..];
                
                // Send private message back to our commander with the message we're about to send to the server
                await client.SendAsync(new PrivMsgMessage(serverMessage.From, $"Sending to server: '{rawMessage}'..."));

                // Send the raw message to the server
                await client.SendRaw(rawMessage);
            }

            // Internal PrivMsgHandler will also handle this message
            // If we don't want the internal handler to be executed, uncomment the next line
            //Handled = true;
            // This might have side-effects, especially for privmsg and channel related commands,
            // since Queries and Channels collection states rely on those messages in Client
        }
    }

    // CommandAttribute not required since the message name is present just before the suffix (Handler) in the class name
    public class NoticeHandler : CustomMessageHandler<NoticeMessage>
    {
        public override Task HandleAsync(NoticeMessage serverMessage, Client client)
        {
            Program.WriteLine($"-{serverMessage.From}- {serverMessage.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
    }

    [Command("375")] // CommandAttribute is required for numeric replies
    public class RplMotdStartHandler : CustomMessageHandler<MotdStartMessage>
    {
        public override Task HandleAsync(MotdStartMessage serverMessage, Client client)
        {
            Program.WriteLine($"MOTD Start: {serverMessage.Text}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
    }

    // Message of the day is not defined in NetIRC, so we create our own
    // It must implement IServerMessage and the constructor needs to have a single parameter of type ParsedIRCMessage
    public class MotdStartMessage : IServerMessage
    {
        public string Text { get; }

        public MotdStartMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
