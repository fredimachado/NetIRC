using Moq;
using NetIRC.Connection;
using NetIRC.Ctcp;
using System;
using Xunit;

namespace NetIRC.Tests
{
    public class ServerMessageTests
    {
        private static User FakeUser = new User("test", "test");
        private readonly Mock<IConnection> mockConnection;
        private readonly Client client;

        public ServerMessageTests()
        {
            mockConnection = new Mock<IConnection>();
            client = new Client(FakeUser, mockConnection.Object);
        }

        [Theory]
        [InlineData("001", "Welcome to the Internet Relay Chat Network NetIRC")]
        [InlineData("002", "Your host is irc.netirc.net, running version 1.0.0")]
        [InlineData("003", "This server was created Mar 30 2021 at 01:00:00")]
        [InlineData("255", "I have 11618 clients and 16 servers")]
        [InlineData("265", "Current local users: 11618 Max: 12530")]
        [InlineData("266", "Current global users: 11618 Max: 12530")]
        public void TestCommonNumericReplies(string command, string message)
        {
            ServerMessage serverMessage = null;

            client.ServerMessages.CollectionChanged += (s, e) => serverMessage = e.NewItems[0] as ServerMessage;

            RaiseDataReceived($":irc.netirc.net {command} NetIRC :{message}");

            Assert.Equal(message, serverMessage.Text);
        }

        [Fact]
        public void TestMyInfoReply()
        {
            var message = "irc.netirc.net ircserver(1.0.0) abcdefGHIJKL mnopqRSTUVXYZ abcDEF";
            ServerMessage serverMessage = null;

            client.ServerMessages.CollectionChanged += (s, e) => serverMessage = e.NewItems[0] as ServerMessage;

            RaiseDataReceived($":irc.netirc.net 004 NetIRC {message}");

            Assert.Equal(message, serverMessage.Text);
        }

        [Fact]
        public void TestISupportReply()
        {
            var support = "CALLERID CASEMAPPING=rfc1459 DEAF=D KICKLEN=180 MODES=4 PREFIX=(qaohv)~&@%+ STATUSMSG=~&@%+ EXCEPTS=e INVEX=I NICKLEN=30";
            var message = "are supported by this server";
            ServerMessage serverMessage = null;

            client.ServerMessages.CollectionChanged += (s, e) => serverMessage = e.NewItems[0] as ServerMessage;

            RaiseDataReceived($":irc.netirc.net 005 NetIRC {support} :{message}");

            Assert.Equal($"{support} {message}", serverMessage.Text);
        }

        [Theory]
        [InlineData("252", 50, "IRC Operators online")]
        [InlineData("253", 1, "unknown connection(s)")]
        [InlineData("254", 999, "channels formed")]
        public void TestLUserReplies(string command, int count, string message)
        {
            ServerMessage serverMessage = null;

            client.ServerMessages.CollectionChanged += (s, e) => serverMessage = e.NewItems[0] as ServerMessage;

            RaiseDataReceived($":irc.netirc.net {command} NetIRC {count} :{message}");

            Assert.Equal($"{count} {message}", serverMessage.Text);
        }

        [Fact]
        public void TestServerMessageTimestamp()
        {
            ServerMessage serverMessage = null;

            client.ServerMessages.CollectionChanged += (s, e) => serverMessage = e.NewItems[0] as ServerMessage;

            RaiseDataReceived($":irc.netirc.net 001 NetIRC :Welcome");

            Assert.Equal(DateTime.Now, serverMessage.Timestamp, TimeSpan.FromSeconds(1));
        }

        private void RaiseDataReceived(string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }
}
