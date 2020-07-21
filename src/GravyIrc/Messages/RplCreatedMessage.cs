using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("003")]
    public class RplCreatedMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplCreatedMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplCreated(new IRCMessageEventArgs<RplCreatedMessage>(this));
        }
    }
}
