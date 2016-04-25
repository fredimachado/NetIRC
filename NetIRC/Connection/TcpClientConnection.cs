using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    public class TcpClientConnection : IConnection
    {
        private TcpSocketClient tcpSocket;

        private const int bufferSize = 4096;
        private const string newLine = "\r\n";

        private string remainingData = string.Empty;

        public TcpClientConnection()
        {
            tcpSocket = new TcpSocketClient();
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public async Task ConnectAsync(string address, int port)
        {
            await tcpSocket.ConnectAsync(address, port);
            RunDataReceiver();
        }

        private async void RunDataReceiver()
        {
            while (true)
            {
                var buffer = new byte[bufferSize];
                int byteCount;

                byteCount = await tcpSocket.ReadStream.ReadAsync(buffer, 0, bufferSize);

                foreach (var line in GetLines(buffer, byteCount))
                {
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(line));
                }
            }
        }

        private IEnumerable<string> GetLines(byte[] buffer, int byteCount)
        {
            if (byteCount == 0)
            {
                return Enumerable.Empty<string>();
            }

            var text = remainingData + Encoding.UTF8.GetString(buffer, 0, byteCount);
            var lines = text.Split(new[] { newLine }, StringSplitOptions.RemoveEmptyEntries);

            if (text.EndsWith(newLine))
            {
                remainingData = string.Empty;
                return lines;
            }

            remainingData = lines.Last();

            return lines.Take(lines.Length - 1);
        }

        public async Task DisconnectAsync()
        {
            await tcpSocket.DisconnectAsync();
        }

        public async Task SendAsync(string data)
        {
            if (!data.EndsWith("\r\n"))
            {
                data += "\r\n";
            }
            var buffer = Encoding.UTF8.GetBytes(data);
            await tcpSocket.WriteStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            tcpSocket.Dispose();
        }
    }
}
