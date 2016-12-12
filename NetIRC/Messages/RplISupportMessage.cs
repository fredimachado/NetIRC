namespace NetIRC.Messages
{
    public class RplISupportMessage : ServerMessage
    {
        public string[] Parameters { get; }
        public string Text { get; }

        public RplISupportMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
            Text = parsedMessage.Trailing;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplISupport(new IRCMessageEventArgs<RplISupportMessage>(this));
        }
    }
}
