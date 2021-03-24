using NetIRC.Messages;
using System;
using System.Threading.Tasks;

namespace NetIRC.ConsoleCli
{
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
}
