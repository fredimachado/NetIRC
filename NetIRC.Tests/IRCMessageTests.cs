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

        [Fact]
        public void NickMessageFromServer()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var parsedIRCMessage = new ParsedIRCMessage($":{oldNick} NICK {newNick}");
            var ircMessage = ServerMessage.Create(parsedIRCMessage);

            Assert.IsType<NickMessage>(ircMessage);

            var nickMessage = ircMessage as NickMessage;
            Assert.Equal(oldNick, nickMessage.OldNick);
            Assert.Equal(newNick, nickMessage.NewNick);
        }

        [Fact]
        public void NickMessageFromClient()
        {
            var newNick = "Kilroy";
            var nickMessage = new NickMessage(newNick);

            Assert.Equal($"NICK {newNick}", nickMessage.ToString());
        }

        [Fact]
        public void UserMessageTokens()
        {
            var user = "guest";
            var realName = "Ronnie Reagan";
            var userMessage = new UserMessage(user, realName);

            Assert.Equal($"USER {user} 0 - :{realName}", userMessage.ToString());
        }
    }
}
