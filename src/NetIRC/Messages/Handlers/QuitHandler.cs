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
                var user = channel.Users.FirstOrDefault(u => u.Nick == serverMessage.Nick);
                if (user != null)
                {
                    channel.Users.Remove(user);
                }
            }

            return Task.CompletedTask;
        }
    }
}
