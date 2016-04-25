using NetIRC.Connection;
using System;
using System.Threading.Tasks;

namespace NetIRC
{
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        public event IRCRawDataHandler OnRawDataReceived;

        public Client(IConnection connection)
        {
            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;
        }

        private void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            OnRawDataReceived?.Invoke(this, e.Data);
        }

        public async Task ConnectAsync(string host, int port, string nick, string user)
        {
            await connection.ConnectAsync(host, port);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
