using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles server ping requests.
    /// </summary>
    public class PingHandler : MessageHandler<PingMessage>
    {
        /// <summary>
        /// Replies with a matching PONG command.
        /// </summary>
        /// <param name="serverMessage">Ping message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A task representing the outgoing send operation.</returns>
        public override Task HandleAsync(PingMessage serverMessage, Client client)
        {
            return client.SendAsync(new PongMessage(serverMessage.Target));
        }
    }
}
