using GravyIrc.Messages;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;

namespace GravyIrc.Tests
{
    public class IRCMessageTests
    {
        [Fact]
        public void PingCommand()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var parsedIRCMessage = new ParsedIrcMessage($"{command} {parameter}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

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
            var parsedIRCMessage = new ParsedIrcMessage($":{prefix} {command} {target} :{text}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var privMsgMessage = Assert.IsType<PrivMsgMessage>(ircMessage);

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
            var parsedIRCMessage = new ParsedIrcMessage($":{from} {command} {target} :{text}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var privMsgMessage = Assert.IsType<NoticeMessage>(ircMessage);

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
            var parsedIRCMessage = new ParsedIrcMessage($":{oldNick} NICK {newNick}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var nickMessage = Assert.IsType<NickMessage>(ircMessage);

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
            var parsedIRCMessage = new ParsedIrcMessage($":{nick} JOIN {channel}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var joinMessage = Assert.IsType<JoinMessage>(ircMessage);

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
        public void PartMessageFromServer()
        {
            var nick = "WiZ";
            var channel = "#NetIRC";
            var parsedIRCMessage = new ParsedIrcMessage($":{nick}!~user@x.y.z PART {channel}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var partMessage = Assert.IsType<PartMessage>(ircMessage);

            Assert.Equal(nick, partMessage.Nick);
            Assert.Equal(channel, partMessage.Channel);
        }

        [Fact]
        public void PartMessageTokens()
        {
            var channel = "#chan";
            var joinMessage = new PartMessage(channel);

            Assert.Equal($"PART {channel}", joinMessage.ToString());
        }

        [Fact]
        public void RplNamReplyMessage()
        {
            var channel = "#NetIRC";
            var nick1 = "NetIRCConsoleClient";
            var nick2 = "Fredi_";
            var parsedIRCMessage = new ParsedIrcMessage($":irc.server.net 353 NetIRCConsoleClient = {channel} :{nick1} @{nick2}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var rplNamReplyMessage = Assert.IsType<RplNamReplyMessage>(ircMessage);

            Assert.Equal(channel, rplNamReplyMessage.Channel);
            Assert.Equal(2, rplNamReplyMessage.Nicks.Count);
            Assert.Equal(nick1, rplNamReplyMessage.Nicks.Keys.ElementAt(0));
            Assert.Equal(nick2, rplNamReplyMessage.Nicks.Keys.ElementAt(1));
            Assert.Equal("@", rplNamReplyMessage.Nicks[nick2]);
        }

        [Fact]
        public void QuitMessageFromServer()
        {
            var nick = "WiZ";
            var message = "Out for lunch";
            var parsedIRCMessage = new ParsedIrcMessage($":{nick}!~user@x.y.z QUIT :{message}");
            var ircMessage = IrcMessage.Create(parsedIRCMessage);

            var quitMessage = Assert.IsType<QuitMessage>(ircMessage);

            Assert.Equal(nick, quitMessage.Nick);
            Assert.Equal(message, quitMessage.Message);
        }

        [Fact]
        public void QuitMessageTokens()
        {
            var message = "Out for lunch";
            var quitMessage = new QuitMessage(message);

            Assert.Equal($"QUIT :{message}", quitMessage.ToString());
        }

        [Fact]
        public void NotClientMessageShouldReturnEmptyToString()
        {
            var testMessage = new TestMessage() as IrcMessage;

            Assert.Equal("TEST", testMessage.ToString());
        }

        [Fact]
        public void ClientMessageWithoutTokensShouldReturnEmptyToString()
        {
            var testMessage = new TestClientMessage();

            Assert.Equal(string.Empty, testMessage.ToString());
        }
    }

    public class TestMessage : IrcMessage
    {
        public override string ToString()
        {
            return "TEST";
        }
    }

    public class TestClientMessage : IrcMessage, IClientMessage
    {
        IEnumerable<string> IClientMessage.Tokens => Enumerable.Empty<string>();
    }
}
