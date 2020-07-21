using System.Collections.Generic;

namespace GravyIrc.Messages
{
    public class UserMessage : IrcMessage, IClientMessage
    {
        public string UserName { get; }
        public string RealName { get; }

        public UserMessage(string userName, string realName)
        {
            UserName = userName;
            RealName = realName;
        }

        public IEnumerable<string> Tokens => new[] { "USER", UserName, "0", "-", RealName };
    }
}
