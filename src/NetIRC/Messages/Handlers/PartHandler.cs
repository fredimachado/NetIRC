using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class PartHandler : MessageHandler<PartMessage>
    {
        public override Task HandleAsync(PartMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            channel.RemoveUser(serverMessage.Nick);

            return Task.CompletedTask;
        }
    }
}
