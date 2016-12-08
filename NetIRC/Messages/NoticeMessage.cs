using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class NoticeMessage : ServerMessage
    {
        public string From { get; }
        public string Target { get; }
        public string Message { get; }

        public NoticeMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Target = parsedMessage.Parameters[0];
            Message = parsedMessage.Text;
        }

        public NoticeMessage(string target, string text)
        {
            Target = target;
            Message = text;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNotice(new IRCMessageEventArgs<NoticeMessage>(this));
        }

        public override IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };
    }
}
