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
        public void TestPingMessage()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var parsedIRCMessage = new ParsedIRCMessage($"{command} {parameter}");

            var pingMessage = new PingMessage(parsedIRCMessage);

            Assert.Equal(parameter, pingMessage.Target);
        }

        [Fact]
        public void TestMessageCreatedDate()
        {
            var parsedIRCMessage = new ParsedIRCMessage("PING 123");

            var pingMessage = new PingMessage(parsedIRCMessage);

            Assert.Equal(DateTime.Now, pingMessage.CreatedDate, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void TestPongMessageTokens()
        {
            var pongMsg = new PongMessage("tolsun.oulu.fi");

            Assert.Equal("PONG tolsun.oulu.fi", pongMsg.ToString());
        }

        [Theory]
        [InlineData("WiZ", "Hello", false)]
        [InlineData("WiZ", "Are you receiving this message ?", false)]
        [InlineData("#NetIRC", "Hello", true)]
        [InlineData("#NetIRC", "Are you receiving this message ?", true)]
        public void TestPrivMsgMessage(string target, string text, bool isChannelMessage)
        {
            var prefix = "Angel!wings@irc.org";
            var raw = $":{prefix} PRIVMSG {target} {(text.Contains(' ') ? $":{text}" : text)}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var privMsgMessage = new PrivMsgMessage(parsedIRCMessage);

            Assert.Equal(prefix, privMsgMessage.Prefix.Raw);
            Assert.Equal(target, privMsgMessage.To);
            Assert.Equal(text, privMsgMessage.Message);
            Assert.Equal(isChannelMessage, privMsgMessage.IsChannelMessage);
        }

        [Theory]
        [InlineData("WiZ", "Hello", false)]
        [InlineData("WiZ", ":Are you receiving this message ?", false)]
        [InlineData("#NetIRC", "Hello", true)]
        [InlineData("#NetIRC", ":Are you receiving this message ?", true)]
        public void TestNoticeMessage(string target, string text, bool isChannelMessage)
        {
            var from = "irc.server.net";
            var raw = $":{from} NOTICE {target} :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var noticeMessage = new NoticeMessage(parsedIRCMessage);

            Assert.Equal(from, noticeMessage.From);
            Assert.Equal(target, noticeMessage.Target);
            Assert.Equal(text, noticeMessage.Message);
            Assert.Equal(isChannelMessage, noticeMessage.IsChannelMessage);
        }

        [Fact]
        public void TestPrivMsgMessageWithNoSpacesAndStartingWithColon()
        {
            var target = "WiZ";
            var message = ":)";
            var privMsgMessage = new PrivMsgMessage(target, message);

            Assert.Equal($"PRIVMSG {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void TestPrivMsgMessageTokens()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new PrivMsgMessage(target, message);

            Assert.Equal($"PRIVMSG {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void TestNoticeMessageTokens()
        {
            var target = "WiZ";
            var message = "Are you receiving this message ?";
            var privMsgMessage = new NoticeMessage(target, message);

            Assert.Equal($"NOTICE {target} :{message}", privMsgMessage.ToString());
        }

        [Fact]
        public void TestNickMessageTokens()
        {
            var newNick = "Kilroy";
            var nickMessage = new NickMessage(newNick);

            Assert.Equal($"NICK {newNick}", nickMessage.ToString());
        }

        [Fact]
        public void TestUserMessageTokens()
        {
            var user = "guest";
            var realName = "Ronnie Reagan";
            var userMessage = new UserMessage(user, realName);

            Assert.Equal($"USER {user} 0 - :{realName}", userMessage.ToString());
        }

        [Fact]
        public void TestJoinMessageTokens()
        {
            var channel = "#chan";
            var joinMessage = new JoinMessage(channel);

            Assert.Equal($"JOIN {channel}", joinMessage.ToString());
        }

        [Fact]
        public void TestJoinMessageWithKeyTokens()
        {
            var channel = "#chan";
            var key = "12345";
            var joinMessage = new JoinMessage(channel, key);

            Assert.Equal($"JOIN {channel} {key}", joinMessage.ToString());
        }

        [Fact]
        public void TestJoinMessageWithMultipleChannelsTokens()
        {
            var channels = new[] { "#chan1", "#chan2", "#chan3" };
            var joinMessage = new JoinMessage(channels);

            Assert.Equal($"JOIN {string.Join(",", channels)}", joinMessage.ToString());
        }

        [Fact]
        public void TestJoinMessageWithMultipleChannelsAndKeysTokens()
        {
            var channelsWithKeys = new Dictionary<string, string>
            {
                { "#chan1", "pass123" },
                { "#chan2", "anotherpass" }
            };
            var joinMessage = new JoinMessage(channelsWithKeys);

            Assert.Equal(
                $"JOIN {string.Join(",", channelsWithKeys.Keys)} {string.Join(",", channelsWithKeys.Values)}",
                joinMessage.ToString());
        }

        [Fact]
        public void TestPartMessageTokens()
        {
            var channel = "#chan";
            var joinMessage = new PartMessage(channel);

            Assert.Equal($"PART {channel}", joinMessage.ToString());
        }

        [Fact]
        public void TestPartMessageWithMultipleChannelsTokens()
        {
            var channels = new[] { "#chan1", "#chan2" };
            var joinMessage = new PartMessage(channels);

            Assert.Equal($"PART {string.Join(",", channels)}", joinMessage.ToString());
        }

        [Fact]
        public void TestQuitMessageTokens()
        {
            var message = "Out for lunch";
            var joinMessage = new QuitMessage(message);

            Assert.Equal($"QUIT :{message}", joinMessage.ToString());
        }

        [Fact]
        public void TestTopicMessageTokens()
        {
            var channel = "#NetIRC";
            var topic = "NetIRC is nice!";
            var topicMessage = new TopicMessage(channel, topic);

            Assert.Equal($"TOPIC {channel} :{topic}", topicMessage.ToString());
        }

        [Fact]
        public void TestClientMessageWithoutTokens_ShouldReturnEmptyToString()
        {
            var testMessage = new TestClientMessage();

            Assert.Equal(string.Empty, testMessage.ToString());
        }

        [Fact]
        public void TestIRCMessageWithoutImplementingIClientMessage_ShouldReturnBaseToString()
        {
            var testMessage = new TestIRCMessage();

            Assert.Equal(typeof(TestIRCMessage).FullName, testMessage.ToString());
        }

        [Fact]
        public void TestModeMessage()
        {
            var prefix = "netIRC!~netIRC@XYZ.IP";
            var target = "netIRC";
            var modes = "+ix";
            var raw = $":{prefix} MODE {target} :{modes}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var modeMessage = new ModeMessage(parsedIRCMessage);

            Assert.Equal(prefix, modeMessage.Prefix.Raw);
            Assert.Equal(target, modeMessage.Target);
            Assert.Equal(modes, modeMessage.Modes);
        }

        [Fact]
        public void TestModeMessageForChannel()
        {
            var prefix = "Fredi!~netIRC@XYZ.IP";
            var target = "#NetIRC";
            var modes = "+o";
            var nick = "NetIRCClient";
            var raw = $":{prefix} MODE {target} {modes} {nick}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var modeMessage = new ModeMessage(parsedIRCMessage);

            Assert.Equal(prefix, modeMessage.Prefix.Raw);
            Assert.Equal(target, modeMessage.Target);
            Assert.Equal(modes, modeMessage.Modes);
            Assert.Equal(nick, modeMessage.Parameters[0]);
        }

        [Fact]
        public void TestModeMessageTokens()
        {
            var target = "WiZ";
            var modes = "+i";

            var modeMessage = new ModeMessage(target, modes);

            Assert.Equal($"MODE {target} {modes}", modeMessage.ToString());
        }

        [Fact]
        public void TestKickMessage()
        {
            var kickedBy = "Fredi";
            var channel = "#netirctest";
            var nick = "NetIRCConsoleClient";
            var raw = $":{kickedBy}!~Fredi@XYZ.IP KICK {channel} {nick}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var kickMessage = new KickMessage(parsedIRCMessage);

            Assert.Equal(kickedBy, kickMessage.KickedBy);
            Assert.Equal(channel, kickMessage.Channel);
            Assert.Equal(nick, kickMessage.Nick);
        }

        [Fact]
        public void TestKickMessageWithComment()
        {
            var kickedBy = "Fredi";
            var channel = "#netirctest";
            var nick = "NetIRCConsoleClient";
            var comment = "I love you!";
            var raw = $":{kickedBy}!~Fredi@XYZ.IP KICK {channel} {nick} :{comment}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var kickMessage = new KickMessage(parsedIRCMessage);

            Assert.Equal(kickedBy, kickMessage.KickedBy);
            Assert.Equal(channel, kickMessage.Channel);
            Assert.Equal(nick, kickMessage.Nick);
            Assert.Equal(comment, kickMessage.Comment);
        }

        [Fact]
        public void TestRplWelcomeMessage()
        {
            var message = "Welcome!";
            var raw = $":irc.netirc.net 001 netIRCTest :{message}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var rplWelcomeMessage = new RplWelcomeMessage(parsedIRCMessage);

            Assert.Equal(message, rplWelcomeMessage.Text);
        }

        [Fact]
        public void TestPassMessageTokens()
        {
            var password = "pass123";

            var passMessage = new PassMessage(password);

            Assert.Equal($"PASS {password}", passMessage.ToString());
        }
    }

    public class TestClientMessage : IRCMessage, IClientMessage
    {
        IEnumerable<string> IClientMessage.Tokens => Enumerable.Empty<string>();
    }

    public class TestIRCMessage : IRCMessage
    {
    }
}
