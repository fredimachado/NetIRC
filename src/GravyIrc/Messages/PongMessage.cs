using System.Collections.Generic;

namespace GravyIrc.Messages
{
    public class PongMessage : IrcMessage, IClientMessage
    {
        public string Target { get; }

        public PongMessage(string target)
        {
            Target = target;
        }

        public IEnumerable<string> Tokens => new[] { "PONG", Target };
    }
}
