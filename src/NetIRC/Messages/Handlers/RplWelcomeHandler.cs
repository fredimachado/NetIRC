using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    [Command("001")]
    public class RplWelcomeHandler : MessageHandler<RplWelcomeMessage>
    {
        public override Task HandleAsync(RplWelcomeMessage serverMessage, Client client)
        {
            client.OnRegistrationCompleted();

            return Task.CompletedTask;
        }
    }
}
