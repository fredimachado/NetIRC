using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class JoinHandler : MessageHandler<JoinMessage>
    {
        public override Task HandleAsync(JoinMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            if (serverMessage.Nick != client.User.Nick)
            {
                var user = client.Peers.GetUser(serverMessage.Nick);
                channel.AddUser(user, string.Empty);
            }

            return Task.CompletedTask;
        }
    }
}
