using GravyIrc.Attributes;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("NOTICE")]
    public class NoticeMessage : IrcMessage, IServerMessage, IClientMessage
    {
        public string From { get; }
        public string Target { get; }
        public string Message { get; }

        public NoticeMessage(ParsedIrcMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Target = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;
        }

        public NoticeMessage(string target, string text)
        {
            Target = target;
            Message = text;
        }

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNotice(new IrcMessageEventArgs<NoticeMessage>(this));
        }

        public IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };
    }
}
