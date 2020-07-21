using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("005")]
    public class RplISupportMessage : IrcMessage, IServerMessage
    {
        public string[] Parameters { get; }
        public string Text { get; }

        public RplISupportMessage(ParsedIrcMessage parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
            Text = parsedMessage.Trailing;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplISupport(new IrcMessageEventArgs<RplISupportMessage>(this));
        }
    }
}
