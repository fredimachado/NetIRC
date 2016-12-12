namespace NetIRC.Messages
{
    public class RplCreatedMessage : ServerMessage
    {
        public string Text { get; }

        public RplCreatedMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplCreated(new IRCMessageEventArgs<RplCreatedMessage>(this));
        }
    }
}
