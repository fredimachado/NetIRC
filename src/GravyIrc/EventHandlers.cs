using GravyIrc.Messages;
using System;

namespace GravyIrc
{
    public delegate void IrcRawDataHandler(Client client, string rawData);
    public delegate void ParsedIrcMessageHandler(Client client, ParsedIrcMessage ircMessage);

    public delegate void IrcMessageEventHandler<T>(Client client, IrcMessageEventArgs<T> e) where T : IrcMessage;

    public class IrcMessageEventArgs<T> : EventArgs where T : IrcMessage
    {
        public T IrcMessage { get; }

        public IrcMessageEventArgs(T ircMessage)
        {
            IrcMessage = ircMessage;
        }
    }
}
