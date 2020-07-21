using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("PING")]
    public class PingMessage : IrcMessage, IServerMessage
    {
        public string Target { get; }

        public PingMessage(ParsedIrcMessage parsedMessage)
        {
            Target = parsedMessage.Trailing ?? parsedMessage.Parameters[0];
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPing(new IrcMessageEventArgs<PingMessage>(this));
        }
    }
}
