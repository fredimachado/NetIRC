using NetIRC.Connection;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Threading;
using System;
using System.Net.Sockets;
using System.Net;

namespace NetIRC.Tests.Connection
{
    public class TcpClientConnectionTests
    {
        private const string HOST = "127.0.0.1";

        private readonly TcpListener tcpListener;
        private readonly int port;

        public TcpClientConnectionTests()
        {
            port = new Random().Next(5000, 15000);
            tcpListener = new TcpListener(IPAddress.Loopback, port);
            tcpListener.Start();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void WhenCreatitngConnectionInstance_ShouldNotProvideNullOrEmptyHost(string host)
        {
            Assert.Throws<ArgumentNullException>(() => new TcpClientConnection(host));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-9999)]
        public void WhenCreatingConnectionInstance_ShouldProvideValidPort(int port)
        {
            Assert.Throws<ArgumentException>(() => new TcpClientConnection(HOST, port));
        }

        [Fact]
        public async Task WhenConnected_TriggerConnectedEvent()
        {
            var pause = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                tcpClient.Connected += (s, e) => pause.Set();

                await tcpClient.ConnectAsync();

                await tcpListener.AcceptTcpClientAsync();
            }

            Assert.True(pause.WaitOne(500));
        }

        [Fact]
        public async Task WhenReceivingData_TriggerDataReceivedEvent()
        {
            var pause = new ManualResetEvent(false);
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                tcpClient.DataReceived += (s, e) =>
                {
                    dataReceived = e.Data;
                    pause.Set();
                };
                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync(data);
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pause.WaitOne(500));
                    }
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact]
        public async Task WhenCatchingException_ShouldTriggerDisconnectedEvent()
        {
            var pause = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                tcpClient.Disconnected += (s, e) => pause.Set();
                tcpClient.DataReceived += (s, e) => throw new Exception();

                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync("test");
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pause.WaitOne(500));
                    }
                }
            }
        }

        [Fact]
        public async Task WhenReceivingDataWithNoDataReceivedHandler_ItShouldBeOK()
        {
            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
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

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
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

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamReader(server.GetStream()))
                    {
                        await tcpClient.SendAsync($"{data}{Constants.CrLf}");
                        dataReceived = await stream.ReadLineAsync();
                    }
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact]
        public async Task WhenServerDisconnects_TrigerDisconnectedEvent()
        {
            var pauseConnected = new ManualResetEvent(false);
            var pauseDisconnected = new ManualResetEvent(false);
            var pauseDataReceived = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                tcpClient.Connected += (s, e) => pauseConnected.Set();
                tcpClient.Disconnected += (s, e) => pauseDisconnected.Set();
                tcpClient.DataReceived += (s, e) => pauseDataReceived.Set();

                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
                {
                    // Wait for the client to be connected if necessary
                    Assert.True(pauseConnected.WaitOne(500));

                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync("test");
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pauseDataReceived.WaitOne(500));
                    }
                }
            }

            Assert.True(pauseDisconnected.WaitOne(500));
        }

        [Fact]
        public async Task WhenServerDisconnects_ShouldBeAbleToReconnect()
        {
            var pauseConnected = new ManualResetEvent(false);
            var pauseDisconnected = new ManualResetEvent(false);
            var pauseDataReceived = new ManualResetEvent(false);

            using (var tcpClient = new TcpClientConnection(HOST, port))
            {
                tcpClient.Connected += (s, e) => pauseConnected.Set();
                tcpClient.Disconnected += (s, e) => pauseDisconnected.Set();
                tcpClient.DataReceived += (s, e) => pauseDataReceived.Set();

                await tcpClient.ConnectAsync();

                using (var server = await tcpListener.AcceptTcpClientAsync())
                {
                    // Wait for the client to be connected if necessary
                    Assert.True(pauseConnected.WaitOne(500));

                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync("test");
                        await stream.FlushAsync();

                        // Wait for the client to receive the data if necessary
                        Assert.True(pauseDataReceived.WaitOne(500));
                    }
                }

                Assert.True(pauseDisconnected.WaitOne(1000));

                await tcpClient.ConnectAsync();
            }
        }
    }
}
