using NetIRC.Messages;
using System.Linq;
using Xunit;
using System;
using System.Collections.Generic;

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
            //var ircMessage = IRCMessage.Create(parsedIRCMessage);

            //Assert.IsType<PingMessage>(ircMessage);
        }

        [Fact]
        public void PongMessageTokens()
        {
            var pongMsg = new PongMessage("tolsun.oulu.fi");

            Assert.Equal("PONG tolsun.oulu.fi", pongMsg.ToString());
        }

        [Fact]
        public void TestPrivMsgMessage()
        {
            var prefix = "Angel!wings@irc.org";
            var target = "WiZ";
            var text = "Are you receiving this message ?";
            var raw = $":{prefix} PRIVMSG {target} :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var privMsgMessage = new PrivMsgMessage(parsedIRCMessage);

            Assert.Equal(prefix, privMsgMessage.Prefix.Raw);
            Assert.Equal(target, privMsgMessage.To);
            Assert.Equal(text, privMsgMessage.Message);
        }

        [Fact]
        public void TestNoticeMessage()
        {
            var from = "irc.server.net";
            var target = "WiZ";
            var text = "Are you receiving this message ?";
            var raw = $":{from} NOTICE {target} :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var noticeMessage = new NoticeMessage(parsedIRCMessage);

            Assert.Equal(from, noticeMessage.From);
            Assert.Equal(target, noticeMessage.Target);
            Assert.Equal(text, noticeMessage.Message);
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
        public void NoticeMessageFromClient()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new NoticeMessage(target, message);

            Assert.Equal($"NOTICE {target} :{message}", privMsgMessage.ToString());
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
        public void PartMessageTokens()
        {
            var channel = "#chan";
            var joinMessage = new PartMessage(channel);

            Assert.Equal($"PART {channel}", joinMessage.ToString());
        }

        [Fact]
        public void QuitMessageTokens()
        {
            var message = "Out for lunch";
            var joinMessage = new QuitMessage(message);

            Assert.Equal($"QUIT :{message}", joinMessage.ToString());
        }

        [Fact]
        public void NotClientMessageShouldReturnEmptyToString()
        {
            var testMessage = new TestMessage() as IRCMessage;

            Assert.Equal("TEST", testMessage.ToString());
        }

        [Fact]
        public void TopicMessageTokens()
        {
            var channel = "#NetIRC";
            var topic = "NetIRC is nice!";
            var topicMessage = new TopicMessage(channel, topic);

            Assert.Equal($"TOPIC {channel} :{topic}", topicMessage.ToString());
        }

        [Fact]
        public void ClientMessageWithoutTokensShouldReturnEmptyToString()
        {
            var testMessage = new TestClientMessage();

            Assert.Equal(string.Empty, testMessage.ToString());
        }
    }

    public class TestMessage : IRCMessage
    {
        public override string ToString()
        {
            return "TEST";
        }
    }

    public class TestClientMessage : IRCMessage, IClientMessage
    {
        IEnumerable<string> IClientMessage.Tokens => Enumerable.Empty<string>();
    }
}
