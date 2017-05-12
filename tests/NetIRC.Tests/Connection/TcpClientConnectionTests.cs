using NetIRC.Connection;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace NetIRC.Tests.Connection
{
    public class TcpClientConnectionTests : IClassFixture<ConnectionFixture>
    {
        private readonly ConnectionFixture connectionFixture;

        public TcpClientConnectionTests(ConnectionFixture connectionFixture)
        {
            this.connectionFixture = connectionFixture;
        }

        [Fact]
        public async Task ConnectsToTheServer()
        {
            var connected = false;

            using (var tcpClient = new TcpClientConnection())
            {
                tcpClient.Connected += (s, e) => connected = true;

                await tcpClient.ConnectAsync("127.0.0.1", 6667);

                await connectionFixture.TcpListener.AcceptTcpClientAsync();
            }

            Assert.True(connected);
        }

        [Fact]
        public async Task ReceivesData()
        {
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection())
            {
                tcpClient.DataReceived += (s, e) => dataReceived = e.Data;
                await tcpClient.ConnectAsync("127.0.0.1", 6667);

                using (var server = await connectionFixture.TcpListener.AcceptTcpClientAsync())
                {
                    using (var stream = new StreamWriter(server.GetStream()))
                    {
                        await stream.WriteLineAsync(data);
                        await stream.FlushAsync();
                    }

                    while (string.IsNullOrEmpty(dataReceived));
                }
            }

            Assert.Equal(data, dataReceived);
        }

        [Fact]
        public async Task SendsData()
        {
            var data = "test";
            var dataReceived = string.Empty;

            using (var tcpClient = new TcpClientConnection())
            {
                await tcpClient.ConnectAsync("127.0.0.1", 6667);

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
    }
}
