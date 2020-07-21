using GravyIrc.Attributes;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("NICK")]
    public class NickMessage : IrcMessage, IServerMessage, IClientMessage
    {
        public string OldNick { get; }
        public string NewNick { get; }

        public NickMessage(ParsedIrcMessage parsedMessage)
        {
            OldNick = parsedMessage.Prefix.From;
            NewNick = parsedMessage.Parameters[0];
        }

        public NickMessage(string newNick)
        {
            NewNick = newNick;
        }

        public IEnumerable<string> Tokens => new[] { "NICK", NewNick };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnNick(new IrcMessageEventArgs<NickMessage>(this));
        }
    }
}
