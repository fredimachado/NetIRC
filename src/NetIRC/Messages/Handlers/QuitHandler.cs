using System.Linq;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles user quit notifications.
    /// </summary>
    public class QuitHandler : MessageHandler<QuitMessage>
    {
        /// <summary>
        /// Removes the quitting user from all tracked channels.
        /// </summary>
        /// <param name="serverMessage">Quit message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(QuitMessage serverMessage, Client client)
        {
            foreach (var channel in client.Channels)
            {
                channel.RemoveUser(serverMessage.Nick);
            }

            return Task.CompletedTask;
        }
    }
}
