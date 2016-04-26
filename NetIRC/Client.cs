using NetIRC.Connection;
using System;
using System.Threading.Tasks;

namespace NetIRC
{
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        public event IRCRawDataHandler OnRawDataReceived;
        public event IRCMessageHandler OnIRCMessageReceived;

        public Client(IConnection connection)
        {
            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;
        }

        private async void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            if (rawData.StartsWith("PING :"))
            {
                await connection.SendAsync("PONG" + rawData.Substring(4));
            }

            var ircMessage = new IRCMessage(rawData);

            OnIRCMessageReceived?.Invoke(this, ircMessage);
        }

        public async Task ConnectAsync(string host, int port, string nick, string user)
        {
            await connection.ConnectAsync(host, port);

            await connection.SendAsync($"NICK {nick}");
            await connection.SendAsync($"USER {nick} 0 - :{user}");
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
