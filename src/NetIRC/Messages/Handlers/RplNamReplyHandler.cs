using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles numeric 353 name reply messages.
    /// </summary>
    [Command("353")]
    public class RplNamReplyHandler : MessageHandler<RplNamReplyMessage>
    {
        /// <summary>
        /// Adds users from the name reply into the target channel.
        /// </summary>
        /// <param name="serverMessage">Name reply message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
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
