﻿namespace NetIRC.Messages
{
    public class RplYourHostMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplYourHostMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
