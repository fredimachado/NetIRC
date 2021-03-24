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
        private const bool verbose = false;

        private static Client client;

        static void Main(string[] args)
        {
            // User connecting to the IRC server
            var user = new User(nickName, "NetIRC");

            // Create IRC client instance, wrapped in a using statement so it gets properly disposed (IDisposable pattern)
            using (client = new Client(user))
            {
                client.OnRawDataReceived += Client_OnRawDataReceived;
                client.Queries.CollectionChanged += Queries_CollectionChanged;

                client.RegistrationCompleted += EventHub_RegistrationCompleted;

                // Handy method to register all custom message handlers in an assembly
                client.RegisterCustomMessageHandlers(typeof(Program).Assembly);

                // Custom message handlers can also be manually registered
                //client.RegisterCustomMessageHandler<PrivMsgHandler>();

                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667));

                Console.Read();
            }
        }

        private static async void EventHub_RegistrationCompleted(object sender, EventArgs e)
        {
            WriteLine("Ready to Roll!", ConsoleColor.Yellow);

            await client.SendAsync(new JoinMessage(channel));
        }

        private static void Queries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Query item in e.NewItems)
            {
                item.Messages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private static void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (ChatMessage item in e.NewItems)
            {
                WriteLine($"<{item.User.Nick}> {item.Text}", ConsoleColor.Green);
            }
        }

        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            if (verbose)
            {
                WriteLine(rawData);
            }
        }

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
