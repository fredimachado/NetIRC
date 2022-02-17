using NetIRC.Connection;
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
        private const string Nick = "JohnD";
        private const string RealName = "John Doe";

        [Fact]
        public async Task WhenConnecting_ClientShouldSendRegistrationMessages()
        {
            var port = 6669;
            var nickMessage = $"NICK {Nick}";
            var userMessage = $"USER {Nick} 0 - :{RealName}";

            var tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();

            var builder = Client.CreateBuilder()
                .WithNick(Nick, RealName)
                .WithServer("localhost", port);

            using var client = builder.Build();

            client.ConnectAsync()
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

            var builder = Client.CreateBuilder()
                .WithNick(Nick, RealName)
                .WithServer("localhost", port, password);

            using var client = builder.Build();

            client.ConnectAsync()
                .SafeFireAndForget(continueOnCapturedContext: false);

            using var server = await tcpListener.AcceptTcpClientAsync();
            using var stream = new StreamReader(server.GetStream());

            Assert.Equal(passMessage, await stream.ReadLineAsync());

            tcpListener.Stop();
        }
    }
}
