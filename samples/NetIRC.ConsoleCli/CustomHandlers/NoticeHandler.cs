using NetIRC.Messages;
using System;
using System.Threading.Tasks;

namespace NetIRC.ConsoleCli
{
    // CommandAttribute not required since the message name is present just before the suffix (Handler) in the class name
    public class NoticeHandler : CustomMessageHandler<NoticeMessage>
    {
        public override Task HandleAsync(NoticeMessage serverMessage, Client client)
        {
            Program.WriteLine($"-{serverMessage.From}- {serverMessage.Message}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
    }
}
