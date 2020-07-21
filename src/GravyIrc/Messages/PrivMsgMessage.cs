using GravyIrc.Attributes;
using System;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("PRIVMSG")]
    public class PrivMsgMessage : IrcMessage, IServerMessage, IClientMessage
    {
        public string From { get; }
        public IrcPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }

        public PrivMsgMessage(ParsedIrcMessage parsedMessage)
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

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPrivMsg(new IrcMessageEventArgs<PrivMsgMessage>(this));
        }

        public bool IsChannelMessage => To[0] == '#';

        public IEnumerable<string> Tokens => new[] { "PRIVMSG", To, Message };
    }
}
