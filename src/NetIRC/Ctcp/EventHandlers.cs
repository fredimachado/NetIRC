using NetIRC.Messages;
using System;

namespace NetIRC.Ctcp
{
    public delegate void CtcpHandler(Client client, CtcpEventArgs ctcpEventArgs);

    public class CtcpEventArgs : EventArgs
    {
        public string From { get; }
        public IRCPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }

        public string CtcpCommand { get; }
        public string CtcpMessage { get; }

        internal CtcpEventArgs(PrivMsgMessage privMsgMessage)
        {
            From = privMsgMessage.From;
            Prefix = privMsgMessage.Prefix;
            To = privMsgMessage.To;
            Message = privMsgMessage.Message;

            var ctcpMessage = Message.Replace(CtcpCommands.CtcpDelimiter, string.Empty);
            if (ctcpMessage.Contains(" "))
            {
                var startIndex = ctcpMessage.IndexOf(' ');
                CtcpCommand = ctcpMessage.Remove(startIndex);
                CtcpMessage = ctcpMessage.Substring(startIndex + 1);
                return;
            }

            CtcpCommand = ctcpMessage;
        }
    }
}
