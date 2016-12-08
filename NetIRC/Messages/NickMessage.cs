using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class NickMessage : ServerMessage
    {
        public string OldNick { get; }
        public string NewNick { get; }

        public NickMessage(ParsedIRCMessage parsedMessage) : base(parsedMessage)
        {
            OldNick = parsedMessage.Prefix.From;
            NewNick = parsedMessage.Parameters[0];
        }

        public NickMessage(string newNick)
        {
            NewNick = newNick;
        }

        public override IEnumerable<string> Tokens => new[] { "NICK", NewNick };

        public override void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNick(new IRCMessageEventArgs<NickMessage>(this));
        }
    }
}
