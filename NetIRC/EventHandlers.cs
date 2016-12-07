using NetIRC.Messages;
using System;

namespace NetIRC
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void ParsedIRCMessageHandler(Client client, ParsedIRCMessage ircMessage);

    public delegate void IRCMessageEventHandler<T>(object sender, IRCMessageEventArgs<T> e) where T : IRCMessage;

    public class IRCMessageEventArgs<T> : EventArgs where T : IRCMessage
    {
        public T IRCMessage { get; }

        public IRCMessageEventArgs(T ircMessage)
        {
            IRCMessage = ircMessage;
        }
    }

    public class BaseIRCEventArgs : EventArgs
    {
        public ParsedIRCMessage IRCMessage { get; }

        public BaseIRCEventArgs(ParsedIRCMessage ircMessage)
        {
            IRCMessage = ircMessage;
        }
    }
}
