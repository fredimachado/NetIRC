using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    public class TcpClientConnection : IConnection
    {
        private readonly TcpClient tcpClient = new TcpClient();

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        private static string crlf = "\r\n";

        public async Task ConnectAsync(string address, int port)
        {
            await tcpClient.ConnectAsync(address, port);

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());

            RunDataReceiver();
        }

        private async void RunDataReceiver()
        {
            while (tcpClient.Connected)
            {
                var line = await streamReader.ReadLineAsync();

                DataReceived?.Invoke(this, new DataReceivedEventArgs(line));
            }
        }

        public async Task SendAsync(string data)
        {
            if (!data.EndsWith(crlf))
            {
                data += crlf;
            }

            await streamWriter.WriteAsync(data);
            await streamWriter.FlushAsync();
        }

        public void Dispose()
        {
            if (streamReader != null)
            {
                streamReader.Dispose();
            }

            if (streamWriter != null)
            {
                streamWriter.Dispose();
            }

            tcpClient.Dispose();
        }
    }
}
