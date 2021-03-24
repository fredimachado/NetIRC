using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class TopicHandler : MessageHandler<TopicMessage>
    {
        public override Task HandleAsync(TopicMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            channel.SetTopic(serverMessage.Topic);

            return Task.CompletedTask;
        }
    }
}
