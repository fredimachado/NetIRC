using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles topic updates for channels.
    /// </summary>
    public class TopicHandler : MessageHandler<TopicMessage>
    {
        /// <summary>
        /// Applies topic text to the target channel.
        /// </summary>
        /// <param name="serverMessage">Topic message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(TopicMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            channel.SetTopic(serverMessage.Topic);

            return Task.CompletedTask;
        }
    }
}
