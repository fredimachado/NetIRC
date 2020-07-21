using System.Collections.Generic;

namespace GravyIrc.Messages
{
    public class ModeMessage : IrcMessage, IClientMessage
    {
        private readonly string mode;
        private readonly string nick;

        public ModeMessage(string nick, string mode)
        {
            this.mode = mode;
            this.nick = nick;
        }

        public IEnumerable<string> Tokens => new[] { "MODE", nick, mode };
    }
}
