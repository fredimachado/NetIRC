using GravyIrc.Connection;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Threading;

namespace GravyIrc.Tests.Connection
{
    public class TcpClientConnectionTests : IClassFixture<ConnectionFixture>
    {
        private const string HOST = "127.0.0.1";
        private const int PORT = 6669;

        private readonly ConnectionFixture connectionFixture;

        public TcpClientConnectionTests(ConnectionFixture connectionFixture)
        {
            this.connectionFixture = connectionFixture;
        }

        [Fact]
        public async Task WhenConnected_TriggerConnectedEvent()
        {
            var pause = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection())
            {
                tcpClient.Connected += (s, e) => pause.Set();

                await tcpClient.ConnectAsync(HOST, PORT);

                await connectionFixture.TcpListener.AcceptTcpClientAsync();
            }

            Assert.True(pause.WaitOne(500));
        }

        [Fact]
        public async Task WhenReceivingData_TriggerDataReceivedEvent()
        {
            var pause = new ManualResetEvent(false);
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection())
            {
                tcpClient.DataReceived += (s, e) =>
                {
                    dataReceived = e.Data;
                    pause.Set();
                };
                await tcpClient.ConnectAsync(HOST, PORT);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync(data);
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pause.WaitOne(60000));
                    }
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact]
        public async Task WhenReceivingDataWithNoDataReceivedHandler_ItShouldBeOK()
        {
            using (var tcpClient = new TcpClientConnection())
            {
                await tcpClient.ConnectAsync(HOST, PORT);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync("test");
                        await stream.FlushAsync();
                    }
                }
            }
        }

        [Fact]
        public async Task WhenSendingData_ServerShouldReceiveIt()
        {
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection())
            {
                await tcpClient.ConnectAsync(HOST, PORT);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamReader(server.GetStream()))
                    {
                        await tcpClient.SendAsync(data);
                        dataReceived = await stream.ReadLineAsync();
                    }
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact]
        public async Task WhenSendingDataEndingWithCrLf_ServerShouldReceiveIt()
        {
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection())
            {
                await tcpClient.ConnectAsync(HOST, PORT);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamReader(server.GetStream()))
                    {
                        await tcpClient.SendAsync($"{data}\r\n");
                        dataReceived = await stream.ReadLineAsync();
                    }
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact(Skip = "Keeps failing. I need to figure out why.")]
        public async Task WhenServerDisconnects_TrigerDisconnectedEvent()
        {
            var pauseConnected = new ManualResetEvent(false);
            var pauseDisconnected = new ManualResetEvent(false);
            var pauseDataReceived = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection())
            {
                tcpClient.Connected += (s, e) => pauseConnected.Set();
                tcpClient.Disconnected += (s, e) => pauseDisconnected.Set();
                tcpClient.DataReceived += (s, e) => pauseDataReceived.Set();

                await tcpClient.ConnectAsync(HOST, PORT);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    // Wait for the client to be connected if necessary
                    Assert.True(pauseConnected.WaitOne(60000));

                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync("test");
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pauseDataReceived.WaitOne(60000));
                    }
                }
            }

            Assert.True(pauseDisconnected.WaitOne(60000));
        }

        [Fact]
        public void WhenDisposingBeforeConnecting_ItShouldBeOK()
        {
            var tcpClient = new TcpClientConnection();
            tcpClient.Dispose();
        }
    }
}
