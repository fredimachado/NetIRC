namespace NetIRC.Messages
{
    public class RplWelcomeMessage : ServerMessage
    {
        public string Text { get; }

        public RplWelcomeMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplWelcome(new IRCMessageEventArgs<RplWelcomeMessage>(this));
        }
    }
}
