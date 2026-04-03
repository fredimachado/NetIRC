using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles user join messages.
    /// </summary>
    public class JoinHandler : MessageHandler<JoinMessage>
    {
        /// <summary>
        /// Tracks users joining a channel.
        /// </summary>
        /// <param name="serverMessage">Join message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(JoinMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            if (serverMessage.Nick != client.User.Nick)
            {
                var user = client.Peers.GetUser(serverMessage.Nick);
                channel.AddUser(user);
            }

            return Task.CompletedTask;
        }
    }
}
