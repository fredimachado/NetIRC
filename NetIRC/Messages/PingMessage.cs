namespace NetIRC.Messages
{
    public class PingMessage : ServerMessage
    {
        public PingMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
        }

        public string Target => parsedMessage.Trailing ?? parsedMessage.Parameters[0];

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPing(new IRCMessageEventArgs<PingMessage>(this));
        }
    }
}
