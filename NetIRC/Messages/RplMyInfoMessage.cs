namespace NetIRC.Messages
{
    public class RplMyInfoMessage : ServerMessage
    {
        public string[] Parameters { get; }

        public RplMyInfoMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplMyInfo(new IRCMessageEventArgs<RplMyInfoMessage>(this));
        }
    }
}
