using NetIRC.Messages;
using Xunit;
using NetIRC.Ctcp;

namespace NetIRC.Tests
{
    public class CtcpMessageTests
    {
        [Fact]
        public void TestCtcpMessage()
        {
            var raw = $":Angel PRIVMSG WiZ :{CtcpCommands.CtcpDelimiter}ACTION likes NetIRC{CtcpCommands.CtcpDelimiter}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var privMsgMessage = new PrivMsgMessage(parsedIRCMessage);

            Assert.True(privMsgMessage.IsCtcp);
        }

        [Fact]
        public void TestCtcpReplyMessageTokens()
        {
            var target = "WiZ";
            var message = "ACTION likes NetIRC";
            var ctcpReplyMessage = new CtcpReplyMessage(target, message);

            Assert.Equal($"NOTICE {target} :{CtcpCommands.CtcpDelimiter}{message}{CtcpCommands.CtcpDelimiter}", ctcpReplyMessage.ToString());
        }
    }
}
