using NetIRC.Ctcp;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class PrivMsgHandler : MessageHandler<PrivMsgMessage>
    {
        public override Task HandleAsync(PrivMsgMessage serverMessage, Client client)
        {
            if (serverMessage.IsCtcp)
            {
                client.OnCtcpReceived(new CtcpEventArgs(serverMessage));
                return Task.CompletedTask;
            }

            var user = client.Peers.GetUser(serverMessage.From);

            if (serverMessage.IsChannelMessage)
            {
                var channel = client.Channels.GetChannel(serverMessage.To);
                var message = new ChannelMessage(user, channel, serverMessage.Message);
                channel.Messages.Add(message);
            }
            else
            {
                var query = client.Queries.GetQuery(user);
                var message = new QueryMessage(user, serverMessage.Message);
                query.Messages.Add(message);
            }

            return Task.CompletedTask;
        }
    }
}
