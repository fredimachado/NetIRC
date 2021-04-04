﻿using NetIRC.Ctcp;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PrivMsgMessage : IRCMessage, IServerMessage, IClientMessage
    {
        public string From { get; }
        public IRCPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }

        public PrivMsgMessage(ParsedIRCMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Prefix = parsedMessage.Prefix;
            To = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;

            IsChannelMessage = To[0] == '#';
            IsCtcp = Message.Contains(CtcpCommands.CtcpDelimiter);
        }

        public PrivMsgMessage(string target, string text)
        {
            To = target;
            Message = !text.Contains(" ") ? $":{text}" : text;
        }

        public bool IsChannelMessage { get; }

        public bool IsCtcp { get; }

        public IEnumerable<string> Tokens => new[] { "PRIVMSG", To, Message };
    }
}
