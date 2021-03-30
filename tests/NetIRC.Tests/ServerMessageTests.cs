using Moq;
using NetIRC.Connection;
using NetIRC.Ctcp;
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
            var received = string.Empty;

            client.ServerMessages.CollectionChanged += (s, e) => received = (e.NewItems[0] as ServerMessage).Text;

            RaiseDataReceived($":irc.netirc.net {command} NetIRC :{message}");

            Assert.Equal(message, received);
        }

        [Fact]
        public void TestMyInfoReply()
        {
            var message = "irc.netirc.net ircserver(1.0.0) abcdefGHIJKL mnopqRSTUVXYZ abcDEF";
            var received = string.Empty;

            client.ServerMessages.CollectionChanged += (s, e) => received = (e.NewItems[0] as ServerMessage).Text;

            RaiseDataReceived($":irc.netirc.net 004 NetIRC {message}");

            Assert.Equal(message, received);
        }

        [Fact]
        public void TestISupportReply()
        {
            var support = "CALLERID CASEMAPPING=rfc1459 DEAF=D KICKLEN=180 MODES=4 PREFIX=(qaohv)~&@%+ STATUSMSG=~&@%+ EXCEPTS=e INVEX=I NICKLEN=30";
            var message = "are supported by this server";
            var received = string.Empty;

            client.ServerMessages.CollectionChanged += (s, e) => received = (e.NewItems[0] as ServerMessage).Text;

            RaiseDataReceived($":irc.netirc.net 005 NetIRC {support} :{message}");

            Assert.Equal($"{support} {message}", received);
        }

        [Theory]
        [InlineData("252", 50, "IRC Operators online")]
        [InlineData("253", 1, "unknown connection(s)")]
        [InlineData("254", 999, "channels formed")]
        public void TestLUserReplies(string command, int count, string message)
        {
            var received = string.Empty;

            client.ServerMessages.CollectionChanged += (s, e) => received = (e.NewItems[0] as ServerMessage).Text;

            RaiseDataReceived($":irc.netirc.net {command} NetIRC {count} :{message}");

            Assert.Equal($"{count} {message}", received);
        }

        private void RaiseDataReceived(string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }
}
