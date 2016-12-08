namespace NetIRC.Messages
{
    public class DefaultIRCMessage : ServerMessage
    {
        public DefaultIRCMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnDefault(new IRCMessageEventArgs<DefaultIRCMessage>(this));
        }
    }
}
