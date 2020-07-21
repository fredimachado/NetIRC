using GravyIrc.Messages;
using System;

namespace GravyIrc
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void ParsedIRCMessageHandler(Client client, ParsedIRCMessage ircMessage);

    public delegate void IRCMessageEventHandler<T>(Client client, IRCMessageEventArgs<T> e) where T : IRCMessage;

    public class IRCMessageEventArgs<T> : EventArgs where T : IRCMessage
    {
        public T IRCMessage { get; }

        public IRCMessageEventArgs(T ircMessage)
        {
            IRCMessage = ircMessage;
        }
    }
}
