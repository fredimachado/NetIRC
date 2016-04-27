using System;

namespace NetIRC
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void IRCMessageHandler(Client client, IRCMessage ircMessage);

    public delegate void PrivMsgHandler(Client client, PrivMsgEventArgs args);

    public class PrivMsgEventArgs : BaseIRCEventArgs
    {
        public string From { get; }
        public IRCPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }

        public PrivMsgEventArgs(IRCMessage ircMessage)
            : base(ircMessage)
        {
            From = ircMessage.Prefix.From;
            Prefix = IRCMessage.Prefix;
            To = ircMessage.Parameters[0];
            Message = ircMessage.Trailing;
        }
    }

    public class BaseIRCEventArgs : EventArgs
    {
        public IRCMessage IRCMessage { get; }

        public BaseIRCEventArgs(IRCMessage ircMessage)
        {
            IRCMessage = ircMessage;
        }
    }
}
