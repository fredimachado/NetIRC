using NetIRC.Messages;
using System.Linq;
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
            var target = "WiZ";
            var text = "Are you receiving this message ?";
            var parsedIRCMessage = new ParsedIRCMessage($":{prefix} {command} {target} :{text}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

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
        public void NoticeMessageFromServer()
        {
            var from = "irc.server.net";
            var command = "NOTICE";
            var target = "WiZ";
            var text = "Are you receiving this message ?";
            var parsedIRCMessage = new ParsedIRCMessage($":{from} {command} {target} :{text}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

            Assert.IsType<NoticeMessage>(ircMessage);

            var privMsgMessage = ircMessage as NoticeMessage;
            Assert.Equal(from, privMsgMessage.From);
            Assert.Equal(target, privMsgMessage.Target);
            Assert.Equal(text, privMsgMessage.Message);
        }

        [Fact]
        public void NoticeMessageFromClient()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new NoticeMessage(target, message);

            Assert.Equal($"NOTICE {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void NickMessageFromServer()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var parsedIRCMessage = new ParsedIRCMessage($":{oldNick} NICK {newNick}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

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

        [Fact]
        public void JoinMessageFromServer()
        {
            var nick = "WiZ";
            var channel = "#Twilight_zone";
            var parsedIRCMessage = new ParsedIRCMessage($":{nick} JOIN {channel}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

            Assert.IsType<JoinMessage>(ircMessage);

            var joinMessage = ircMessage as JoinMessage;
            Assert.Equal(nick, joinMessage.Nick);
            Assert.Equal(channel, joinMessage.Channel);
        }

        [Fact]
        public void JoinMessageTokens()
        {
            var channel = "#chan";
            var joinMessage = new JoinMessage(channel);

            Assert.Equal($"JOIN {channel}", joinMessage.ToString());
        }

        [Fact]
        public void JoinMessageWithKeyTokens()
        {
            var channel = "#chan";
            var key = "12345";
            var joinMessage = new JoinMessage(channel, key);

            Assert.Equal($"JOIN {channel} {key}", joinMessage.ToString());
        }

        [Fact]
        public void RplNamReplyMessage()
        {
            var channel = "#NetIRC";
            var nick1 = "NetIRCConsoleClient";
            var nick2 = "Fredi_";
            var parsedIRCMessage = new ParsedIRCMessage($":irc.server.net 353 NetIRCConsoleClient = {channel} :{nick1} @{nick2}");
            var ircMessage = IRCMessage.Create(parsedIRCMessage);

            Assert.IsType<RplNamReplyMessage>(ircMessage);

            var rplNamReplyMessage = ircMessage as RplNamReplyMessage;
            Assert.Equal(channel, rplNamReplyMessage.Channel);
            Assert.Equal(2, rplNamReplyMessage.Nicks.Count);
            Assert.Equal(nick1, rplNamReplyMessage.Nicks.Keys.ElementAt(0));
            Assert.Equal(nick2, rplNamReplyMessage.Nicks.Keys.ElementAt(1));
            Assert.Equal("@", rplNamReplyMessage.Nicks[nick2]);
        }
    }
}
