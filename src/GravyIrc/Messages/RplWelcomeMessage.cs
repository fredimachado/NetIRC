﻿using GravyIrc.Attributes;

namespace GravyIrc.Messages
{
    [ServerMessage("001")]
    public class RplWelcomeMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplWelcomeMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnRplWelcome(new IRCMessageEventArgs<RplWelcomeMessage>(this));
        }
    }
}
