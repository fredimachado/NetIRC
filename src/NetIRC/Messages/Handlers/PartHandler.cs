using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles channel part messages.
    /// </summary>
    public class PartHandler : MessageHandler<PartMessage>
    {
        /// <summary>
        /// Removes a user from the specified channel.
        /// </summary>
        /// <param name="serverMessage">Part message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(PartMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            channel.RemoveUser(serverMessage.Nick);

            return Task.CompletedTask;
        }
    }
}
