using System.Collections.Generic;
using System.Linq;

namespace NetIRC.Messages
{
    public class ModeMessage : IRCMessage, IServerMessage, IClientMessage
    {
        public IRCPrefix Prefix { get; }
        public string Target { get; }
        public string Modes { get; }
        public string[] Parameters { get; }

        public ModeMessage(ParsedIRCMessage parsedMessage)
        {
            Prefix = parsedMessage.Prefix;
            Target = parsedMessage.Parameters[0];
            Modes = parsedMessage.Parameters[1] ?? parsedMessage.Trailing;
            if (parsedMessage.Parameters.Length > 2)
            {
                Parameters = parsedMessage.Parameters
                    .Skip(2)
                    .ToArray();
            }
        }

        public ModeMessage(string target, string modes)
        {
            Target = target;
            Modes = modes;
        }

        public IEnumerable<string> Tokens => new[] { "MODE", Target, Modes };
    }
}
