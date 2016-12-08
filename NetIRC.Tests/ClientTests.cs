﻿using Moq;
using NetIRC.Connection;
using NetIRC.Messages;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class ClientTests
    {
        [Fact]
        public void TriggersRawDataReceived()
        {
            var raw = "PING xyz.com";
            var rawReceived = string.Empty;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.OnRawDataReceived += (c, d) => rawReceived = d;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(raw, rawReceived);
        }

        [Fact]
        public void RespondsToPingMessage()
        {
            var data = "xyz.com";
            var raw = $"PING :{data}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            mockConnection.Verify(c => c.SendAsync($"PONG {data}"), Times.Once());
        }

        [Fact]
        public void TriggersOnPingEvent()
        {
            var raw = "PING :xyz.com";
            IRCMessageEventArgs<PingMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.EventHub.Ping += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal("xyz.com", args.IRCMessage.Target);
        }

        [Fact]
        public async Task SendsNickAndUserWhenConnected()
        {
            var nick = "guest";
            var realName = "Ronnie Reagan";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            await Task.Run(() => client.ConnectAsync("localhost", 6667, nick, realName));

            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{realName}"));
        }

        [Fact]
        public void TriggersIRCMessageReceived()
        {
            var raw = ":irc.rizon.io 439 * :Please wait while we process your connection.";
            ParsedIRCMessage ircMessage = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.OnIRCMessageParsed += (c, m) => ircMessage = m;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal("irc.rizon.io", ircMessage.Prefix.From);
            Assert.Equal("439", ircMessage.Command);
            Assert.Equal("*", ircMessage.Parameters[0]);
            Assert.Equal("Please wait while we process your connection.", ircMessage.Trailing);
        }

        [Fact]
        public void TriggersOnPrivMsgReceived()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {to} :{message}";
            IRCMessageEventArgs<PrivMsgMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.EventHub.PrivMsg += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(from, args.IRCMessage.Prefix.From);
            Assert.Equal(to, args.IRCMessage.To);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnPrivMsgReceivedWithoutTrailing()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello";
            var raw = $":{from} PRIVMSG {to} {message}";
            IRCMessageEventArgs<PrivMsgMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.EventHub.PrivMsg += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(from, args.IRCMessage.Prefix.From);
            Assert.Equal(to, args.IRCMessage.To);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnNoticeReceived()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello world";
            var raw = $":{from} NOTICE {to} :{message}";
            IRCMessageEventArgs<NoticeMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.EventHub.Notice += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(to, args.IRCMessage.Target);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnNoticeReceivedWithoutTrailing()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello";
            var raw = $":{from} NOTICE {to} {message}";
            IRCMessageEventArgs<NoticeMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.EventHub.Notice += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(to, args.IRCMessage.Target);
            Assert.Equal(message, args.IRCMessage.Message);
        }
    }
}
