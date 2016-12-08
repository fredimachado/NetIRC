using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class PongCommand : IRCMessage
    {
        public string Target { get; }

        public PongCommand(string target)
        {
            Target = target;
        }

        public override void TriggerEvent(EventHub eventHub)
        {
        }

        public override IEnumerable<string> Tokens => new[] { "PONG", Target };
    }
}
