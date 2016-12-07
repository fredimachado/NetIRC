using System;

namespace NetIRC.Messages
{
    public class PingCommand : IRCMessage
    {
        public PingCommand(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
        }

        public string Target => parsedMessage.Trailing ?? parsedMessage.Parameters[0];

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPing(new IRCMessageEventArgs<PingCommand>(this));
        }
    }
}
