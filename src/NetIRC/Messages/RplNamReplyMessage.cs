using System;
using System.Collections.Generic;
using System.Linq;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents server numeric 353 (name reply) message.
    /// </summary>
    public class RplNamReplyMessage : IRCMessage, IServerMessage
    {
        /// <summary>
        /// Gets the channel associated with the reply.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets nicknames and their optional status prefixes.
        /// </summary>
        public Dictionary<string, string> Nicks { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="RplNamReplyMessage"/>.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
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
