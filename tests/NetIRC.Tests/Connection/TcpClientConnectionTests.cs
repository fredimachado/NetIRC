using NetIRC.Connection;
using System.Threading.Tasks;
using Xunit;
using System.Net.Sockets;
using System.Net;
using System.Text;
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

            var tcpClient = new TcpClientConnection();
            tcpClient.Connected += (s, e) => connected = true;

            await tcpClient.ConnectAsync("127.0.0.1", 6667);

            await connectionFixture.TcpListener.AcceptTcpClientAsync();

            Assert.True(connected);
        }

        [Fact]
        public async Task ReceivesData()
        {
            var data = "test";
            var dataReceived = string.Empty;

            var tcpClient = new TcpClientConnection();
            tcpClient.DataReceived += (s, e) => dataReceived = e.Data;
            await tcpClient.ConnectAsync("127.0.0.1", 6667);

            var server = await connectionFixture.TcpListener.AcceptTcpClientAsync();
            var stream = new StreamWriter(server.GetStream());
            await stream.WriteLineAsync(data);
            await stream.FlushAsync();

            while (dataReceived == string.Empty);

            Assert.Equal(data, dataReceived);
        }
    }
}
