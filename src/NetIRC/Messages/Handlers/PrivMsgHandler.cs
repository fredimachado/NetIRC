using NetIRC.Ctcp;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles PRIVMSG messages for channels, queries, and CTCP.
    /// </summary>
    public class PrivMsgHandler : MessageHandler<PrivMsgMessage>
    {
        /// <summary>
        /// Dispatches private messages to channel/query storage or CTCP processing.
        /// </summary>
        /// <param name="serverMessage">Private message payload.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
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
                Client.DispatcherInvoker.Invoke(() => channel.Messages.Add(message));
            }
            else
            {
                var query = client.Queries.GetQuery(user);
                var message = new QueryMessage(user, serverMessage.Message);
                Client.DispatcherInvoker.Invoke(() => query.Messages.Add(message));
            }

            return Task.CompletedTask;
        }
    }
}
