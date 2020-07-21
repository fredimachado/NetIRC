using GravyIrc.Attributes;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("JOIN")]
    public class JoinMessage : IrcMessage, IServerMessage, IClientMessage
    {
        private string channels;
        private string keys;


        public string Nick { get; }
        public string Channel { get; }

        public JoinMessage(ParsedIrcMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
        }

        public JoinMessage(string channels, string keys = "")
        {
            this.channels = channels;
            this.keys = keys;
        }

        public IEnumerable<string> Tokens => new[] { "JOIN", channels, keys };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnJoin(new IrcMessageEventArgs<JoinMessage>(this));
        }
    }
}
