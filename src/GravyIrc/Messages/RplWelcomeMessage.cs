using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("001")]
    public class RplWelcomeMessage : IrcMessage, IServerMessage
    {
        public string Text { get; }

        public RplWelcomeMessage(ParsedIrcMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplWelcome(new IrcMessageEventArgs<RplWelcomeMessage>(this));
        }
    }
}
