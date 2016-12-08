using NetIRC.Messages;
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
            var ircMessage = ServerMessage.Create(parsedIRCMessage);

            Assert.IsType<PingMessage>(ircMessage);
        }

        [Fact]
        public void PongMessageTokens()
        {
            var pongMsg = new PongMessage("tolsun.oulu.fi");

            Assert.Equal("PONG tolsun.oulu.fi", pongMsg.ToString());
        }

        [Fact]
        public void PrivMsgMessageFromServer()
        {
            var prefix = "Angel!wings@irc.org";
            var command = "PRIVMSG";
            var target = "Wiz";
            var text = "Are you receiving this message ?";
            var parsedIRCMessage = new ParsedIRCMessage($":{prefix} {command} {target} :{text}");
            var ircMessage = ServerMessage.Create(parsedIRCMessage);

            Assert.IsType<PrivMsgMessage>(ircMessage);

            var privMsgMessage = ircMessage as PrivMsgMessage;
            Assert.Equal(prefix, privMsgMessage.Prefix.Raw);
            Assert.Equal(target, privMsgMessage.To);
            Assert.Equal(text, privMsgMessage.Message);
        }

        [Fact]
        public void PrivMsgMessageFromClient()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new PrivMsgMessage(target, message);

            Assert.Equal($"PRIVMSG {target} :{message}", privMsgMessage.ToString());
        }
        }
    }
}
