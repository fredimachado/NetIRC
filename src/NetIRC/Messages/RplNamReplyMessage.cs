using System;
using System.Collections.Generic;
using System.Linq;

namespace NetIRC.Messages
{
    public class RplNamReplyMessage : IRCMessage, IServerMessage
    {
        public string Channel { get; }
        public Dictionary<string, string> Nicks { get; }

        public RplNamReplyMessage(ParsedIRCMessage parsedMessage)
        {
            Nicks = new Dictionary<string, string>();

            Channel = parsedMessage.Parameters[2];
            var nicks = parsedMessage.Trailing.Split(' ');

            foreach (var nick in nicks)
            {
                if (!string.IsNullOrWhiteSpace(nick) && NetIRC.Channel.UserStatuses.Contains(nick[0]))
                {
                    Nicks.Add(nick.Substring(1), nick.Substring(0, 1));
                }
                else
                {
                    Nicks.Add(nick, string.Empty);
                }
            }
        }
    }
}
