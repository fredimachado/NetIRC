﻿using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("002")]
    public class RplYourHostMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplYourHostMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplYourHost(new IRCMessageEventArgs<RplYourHostMessage>(this));
        }
    }
}
