using GravyIrc.Attributes;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("PART")]
    public class PartMessage : IrcMessage, IServerMessage, IClientMessage
    {
        private string channels;


        public string Nick { get; }
        public string Channel { get; }

        public PartMessage(ParsedIrcMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        public PartMessage(string channels)
        {
            this.channels = channels;
        }

        public IEnumerable<string> Tokens => new[] { "PART", channels };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnPart(new IrcMessageEventArgs<PartMessage>(this));
        }
    }
}
