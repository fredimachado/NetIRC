using NetIRC.Messages;
using System;

namespace NetIRC.Ctcp
{
    /// <summary>
    /// Represents a callback invoked when a CTCP message is received.
    /// </summary>
    /// <param name="client">The client that received the message.</param>
    /// <param name="ctcpEventArgs">The parsed CTCP event arguments.</param>
    public delegate void CtcpHandler(Client client, CtcpEventArgs ctcpEventArgs);

    /// <summary>
    /// Provides details for a received CTCP message.
    /// </summary>
    public class CtcpEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the sender nickname.
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Gets the sender prefix information.
        /// </summary>
        public IRCPrefix Prefix { get; }

        /// <summary>
        /// Gets the CTCP target nickname or channel.
        /// </summary>
        public string To { get; }

        /// <summary>
        /// Gets the original CTCP-framed message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the parsed CTCP command token.
        /// </summary>
        public string CtcpCommand { get; }

        /// <summary>
        /// Gets the CTCP command payload text when present.
        /// </summary>
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
