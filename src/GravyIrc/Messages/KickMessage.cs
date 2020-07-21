using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("KICK")]
    public class KickMessage : IrcMessage, IServerMessage
    {
        public string Channel { get; }
        public string Nick { get; set; }
        public string KickedBy { get; set; }

        public KickMessage(ParsedIrcMessage parsedMessage)
        {
            Channel = parsedMessage.Parameters[0];
            Nick = parsedMessage.Parameters[1];
            KickedBy = parsedMessage.Parameters[2];
        }

        public KickMessage(string channel)
        {
            Channel = channel;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnKick(new IrcMessageEventArgs<KickMessage>(this));
        }
    }
}
