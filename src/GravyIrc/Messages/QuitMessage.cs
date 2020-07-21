using GravyIrc.Attributes;
using System.Collections.Generic;

namespace GravyIrc.Messages
{
    [ServerMessage("QUIT")]
    public class QuitMessage : IrcMessage, IServerMessage, IClientMessage
    {
        public string Nick { get; }
        public string Message { get; }

        public QuitMessage(ParsedIrcMessage parsedMessage)
        {
            Nick = parsedMessage.Prefix.From;
            Message = parsedMessage.Trailing;
        }

        public QuitMessage(string message)
        {
            Message = message;
        }

        public IEnumerable<string> Tokens => new[] { "QUIT", Message };

        public void TriggerEvent(EventHub eventHub)
        {
            eventHub.OnQuit(new IrcMessageEventArgs<QuitMessage>(this));
        }
    }
}
