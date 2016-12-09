using System;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PrivMsgMessage : ServerMessage
    {
        public string From { get; }
        public IRCPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }

        public PrivMsgMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Prefix = parsedMessage.Prefix;
            To = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;
        }

        public PrivMsgMessage(string target, string text)
        {
            To = target;
            Message = text;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPrivMsg(new IRCMessageEventArgs<PrivMsgMessage>(this));
        }

        public override IEnumerable<string> Tokens => new[] { "PRIVMSG", To, Message };
    }
}
