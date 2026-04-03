using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles nickname change messages.
    /// </summary>
    public class NickHandler : MessageHandler<NickMessage>
    {
        /// <summary>
        /// Updates tracked peer nickname values.
        /// </summary>
        /// <param name="serverMessage">Nick change message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(NickMessage serverMessage, Client client)
        {
            var user = client.Peers.GetUser(serverMessage.OldNick);
            user.Nick = serverMessage.NewNick;

            return Task.CompletedTask;
        }
    }
}
