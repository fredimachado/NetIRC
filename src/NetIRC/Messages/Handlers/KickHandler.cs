using System;
using System.Threading.Tasks;

namespace NetIRC.Messages.Handlers
{
    public class KickHandler : MessageHandler<KickMessage>
    {
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
