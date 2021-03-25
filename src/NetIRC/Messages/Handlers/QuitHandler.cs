using System.Linq;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class QuitHandler : MessageHandler<QuitMessage>
    {
        public override Task HandleAsync(QuitMessage serverMessage, Client client)
        {
            foreach (var channel in client.Channels)
            {
                channel.RemoveUser(serverMessage.Nick);
            }

            return Task.CompletedTask;
        }
    }
}
