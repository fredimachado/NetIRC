using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    [Command("353")]
    public class RplNamReplyHandler : MessageHandler<RplNamReplyMessage>
    {
        public override Task HandleAsync(RplNamReplyMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            foreach (var nick in serverMessage.Nicks)
            {
                var user = client.Peers.GetUser(nick.Key);
                if (channel.GetUser(nick.Key) is null)
                {
                    channel.AddUser(user, nick.Value);
                }
            }

            return Task.CompletedTask;
        }
    }
}
