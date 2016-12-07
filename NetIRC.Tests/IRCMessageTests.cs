using NetIRC.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class IRCMessageTests
    {
        [Fact]
        public void PingCommand()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var parsedIRCMessage = new ParsedIRCMessage($"{command} {parameter}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

            Assert.IsType<PingCommand>(ircMessage);
        }

        [Fact]
        public void PrivMsgMessage()
        {
            var prefix = "Angel!wings@irc.org";
            var command = "PRIVMSG";
            var target = "Wiz";
            var text = "Are you receiving this message ?";
            var parsedIRCMessage = new ParsedIRCMessage($":{prefix} {command} {target} :{text}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

            Assert.IsType<PrivMsgMessage>(ircMessage);

            var privMsgMessage = ircMessage as PrivMsgMessage;
            Assert.Equal(prefix, privMsgMessage.Prefix.Raw);
            Assert.Equal(target, privMsgMessage.To);
            Assert.Equal(text, privMsgMessage.Message);
        }
    }
}
