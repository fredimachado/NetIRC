using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class PrivMsgHandler : MessageHandler<PrivMsgMessage>
    {
        public override Task HandleAsync(PrivMsgMessage serverMessage, Client client)
        {
            var user = client.Peers.GetUser(serverMessage.From);
            var message = new ChatMessage(user, serverMessage.Message);

            if (serverMessage.IsChannelMessage)
            {
                var channel = client.Channels.GetChannel(serverMessage.To);
                channel.Messages.Add(message);
            }
            else
            {
                var query = client.Queries.GetQuery(user);
                query.Messages.Add(message);
            }

            return Task.CompletedTask;
        }
    }
}
