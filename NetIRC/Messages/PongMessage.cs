using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PongMessage : IRCMessage
    {
        public string Target { get; }

        public PongMessage(string target)
        {
            Target = target;
        }

        public override IEnumerable<string> Tokens => new[] { "PONG", Target };
    }
}
