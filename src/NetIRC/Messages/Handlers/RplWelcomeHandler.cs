using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles numeric 001 welcome replies.
    /// </summary>
    [Command("001")]
    public class RplWelcomeHandler : MessageHandler<RplWelcomeMessage>
    {
        /// <summary>
        /// Marks registration as completed for the current client.
        /// </summary>
        /// <param name="serverMessage">Welcome message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(RplWelcomeMessage serverMessage, Client client)
        {
            client.OnRegistrationCompleted();

            return Task.CompletedTask;
        }
    }
}
