using System;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class DefaultIRCMessage : IRCMessage
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
