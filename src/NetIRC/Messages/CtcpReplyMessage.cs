using NetIRC.Ctcp;
using System.Collections.Generic;

namespace NetIRC.Messages
{
    public class CtcpReplyMessage : IClientMessage
    {
        public string Target { get; }
        public string Message { get; }

        public CtcpReplyMessage(string target, string text)
        {
            Target = target;
            Message = $":{CtcpCommands.CtcpDelimiter}{text}{CtcpCommands.CtcpDelimiter}";
        }

        public IEnumerable<string> Tokens => new[] { "NOTICE", Target, Message };

        public override string ToString() => string.Join(" ", Tokens);
    }
}
