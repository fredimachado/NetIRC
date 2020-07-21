using System.Collections.Generic;

namespace GravyIrc.Messages
{
    public class PassMessage : IrcMessage, IClientMessage
    {
        private readonly string password;

        public PassMessage(string password)
        {
            this.password = password;
        }

        public IEnumerable<string> Tokens => new[] { "PASS", password };
    }
}
