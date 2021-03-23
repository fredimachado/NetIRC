using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class PingHandler : MessageHandler<PingMessage>
    {
        public override Task HandleAsync(PingMessage serverMessage, Client client)
        {
            return client.SendAsync(new PongMessage(serverMessage.Target));
        }
    }
}
