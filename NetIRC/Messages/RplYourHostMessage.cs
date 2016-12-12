namespace NetIRC.Messages
{
    public class RplYourHostMessage : ServerMessage
    {
        public string Text { get; }

        public RplYourHostMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplYourHost(new IRCMessageEventArgs<RplYourHostMessage>(this));
        }
    }
}
