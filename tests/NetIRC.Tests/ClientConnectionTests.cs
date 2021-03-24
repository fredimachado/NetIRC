using NetIRC.Extensions;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace NetIRC.Tests
{
    public class ClientConnectionTests
    {
        private static User FakeUser = new User("test", "test");

        [Fact]
        public async Task WhenConnecting_ClientShouldSendRegistrationMessages()
        {
            var port = 6669;
            var nickMessage = $"NICK {FakeUser.Nick}";
            var userMessage = $"USER {FakeUser.Nick} 0 - {FakeUser.RealName}";

            var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();

            using var client = new Client(FakeUser);

            client.ConnectAsync("localhost", port)
                .SafeFireAndForget(continueOnCapturedContext: false);

            using var server = await tcpListener.AcceptTcpClientAsync();
            using var stream = new StreamReader(server.GetStream());

            Assert.Equal(nickMessage, await stream.ReadLineAsync());
            Assert.Equal(userMessage, await stream.ReadLineAsync());

            tcpListener.Stop();
        }

        [Fact]
        public async Task WhenConnectingWithPassword_ClientShouldSendPassMessage()
        {
            var port = 6670;
            var password = "passw0rd123";
            var passMessage = $"PASS {password}";

            var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();

            using var client = new Client(FakeUser, password);

            client.ConnectAsync("localhost", port)
                .SafeFireAndForget(continueOnCapturedContext: false);

            using var server = await tcpListener.AcceptTcpClientAsync();
            using var stream = new StreamReader(server.GetStream());

            Assert.Equal(passMessage, await stream.ReadLineAsync());

            tcpListener.Stop();
        }
    }
}
