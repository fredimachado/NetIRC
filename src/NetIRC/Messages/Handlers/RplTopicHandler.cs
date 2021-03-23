using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    [Command("332")]
    public class RplTopicHandler : MessageHandler<RplTopicMessage>
    {
        public override Task HandleAsync(RplTopicMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            channel.SetTopic(serverMessage.Topic);

            return Task.CompletedTask;
        }
    }
}
