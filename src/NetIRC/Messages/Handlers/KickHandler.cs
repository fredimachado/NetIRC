using System;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    /// <summary>
    /// Handles kick messages for channels.
    /// </summary>
    public class KickHandler : MessageHandler<KickMessage>
    {
        /// <summary>
        /// Removes the kicked user or channel from tracked collections.
        /// </summary>
        /// <param name="serverMessage">Kick message.</param>
        /// <param name="client">Target IRC client.</param>
        /// <returns>A completed task.</returns>
        public override Task HandleAsync(KickMessage serverMessage, Client client)
        {
            var channel = client.Channels.GetChannel(serverMessage.Channel);
            if (channel != null)
            {
                if (string.Equals(serverMessage.Nick, client.User.Nick, StringComparison.InvariantCultureIgnoreCase))
                {
                    client.Channels.Remove(channel);
                }
                else
                {
                    channel.RemoveUser(serverMessage.Nick);
                }
            }

            return Task.CompletedTask;
        }
    }
}
