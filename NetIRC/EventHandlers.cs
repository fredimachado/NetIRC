using System;

namespace NetIRC
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void IRCMessageHandler(Client client, IRCMessage ircMessage);

    public delegate void PrivMsgHandler(Client client, PrivMsgEventArgs args);

    public class PrivMsgEventArgs : EventArgs
    {
        public string From { get; }
        public string To { get; }
        public string Message { get; }

        public PrivMsgEventArgs(IRCMessage ircMessage)
        {
            From = ircMessage.Prefix.From;
            To = ircMessage.Parameters[0];
            Message = ircMessage.Trailing;
        }
    }
}
