using NetIRC.Messages;
using System;
using System.Threading.Tasks;

namespace NetIRC.ConsoleCli
{
    [Command("375")] // CommandAttribute is required for numeric replies
    public class RplMotdStartHandler : CustomMessageHandler<MotdStartMessage>
    {
        public override Task HandleAsync(MotdStartMessage serverMessage, Client client)
        {
            Program.WriteLine($"MOTD Start: {serverMessage.Text}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
    }
}
