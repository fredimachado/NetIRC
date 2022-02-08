using Moq;
using NetIRC.Connection;
using NetIRC.Ctcp;
using Xunit;

namespace NetIRC.Tests
{
    public class CtcpCommandReplyTests
    {
        private static readonly User FakeUser = new User("test", "test");
        private readonly Mock<IConnection> mockConnection;
        private readonly Client client;

        public CtcpCommandReplyTests()
        {
            mockConnection = new Mock<IConnection>();
            client = new Client(FakeUser, mockConnection.Object);
        }

        [Fact]
        public void TestClientInfoReply()
        {
            RaiseDataReceived($":Angel PRIVMSG WiZ :{CtcpWrapper("CLIENTINFO")}");

            var expected = $"NOTICE Angel :{CtcpWrapper("CLIENTINFO ACTION CLIENTINFO PING TIME VERSION")}";
            mockConnection.Verify(c => c.SendAsync(expected), Times.Once);
        }

        [Fact]
        public void TestPingReply()
        {
            RaiseDataReceived($":Angel PRIVMSG WiZ :{CtcpWrapper("PING 1234")}");

            var expected = $"NOTICE Angel :{CtcpWrapper("PING 1234")}";
            mockConnection.Verify(c => c.SendAsync(expected), Times.Once);
        }

        [Fact]
        public void TestTimeReply()
        {
            RaiseDataReceived($":Angel PRIVMSG WiZ :{CtcpWrapper("TIME")}");

            mockConnection.Verify(c => c.SendAsync(It.Is<string>(s => s.StartsWith("NOTICE") && s.Contains("TIME"))), Times.Once);
        }

        [Fact]
        public void TestVersionReply()
        {
            RaiseDataReceived($":Angel PRIVMSG WiZ :{CtcpWrapper("VERSION")}");

            mockConnection.Verify(c => c.SendAsync(It.Is<string>(s => s.Contains("NetIRC"))), Times.Once);
        }

        private string CtcpWrapper(string message) => $"{CtcpCommands.CtcpDelimiter}{message}{CtcpCommands.CtcpDelimiter}";

        private void RaiseDataReceived(string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }
}
