using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class NickHandler : MessageHandler<NickMessage>
    {
        public override Task HandleAsync(NickMessage serverMessage, Client client)
        {
            var user = client.Peers.GetUser(serverMessage.OldNick);
            user.Nick = serverMessage.NewNick;

            return Task.CompletedTask;
        }
    }
}
