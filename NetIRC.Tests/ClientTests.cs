using Moq;
using NetIRC.Connection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class ClientTests
    {
        [Fact]
        public void TriggersRawDataReceived()
        {
            var raw = "PING";
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

            mockConnection.Verify(c => c.SendAsync($"PONG :{data}"), Times.Once());
        }

        [Fact]
        public async Task SendsNickAndUserWhenConnected()
        {
            var nick = "test";
            var user = "user";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            await Task.Run(() => client.ConnectAsync("localhost", 6667, nick, user));

            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{user}"));
        }

        [Fact]
        public void TriggersIRCMessageReceived()
        {
            var raw = ":irc.rizon.io 439 * :Please wait while we process your connection.";
            IRCMessage ircMessage = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.OnIRCMessageReceived += (c, m) => ircMessage = m;

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
            PrivMsgEventArgs args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.OnPrivMsgReceived += (c, a) => args = a;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(from, args.From);
            Assert.Equal(to, args.To);
            Assert.Equal(message, args.Message);
        }
    }
}
