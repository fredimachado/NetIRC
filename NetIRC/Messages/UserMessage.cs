using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class UserMessage : IRCMessage
    {
        public string UserName { get; }
        public string RealName { get; }

        public UserMessage(string userName, string realName)
        {
            UserName = userName;
            RealName = realName;
        }

        public override IEnumerable<string> Tokens => new[] { "USER", UserName, "0", "-", RealName };
    }
}
