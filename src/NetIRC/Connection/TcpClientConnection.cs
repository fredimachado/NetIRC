using NetIRC.Extensions;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    /// <summary>
    /// Represents a TCP connection to an IRC server
    /// </summary>
    public class TcpClientConnection : IConnection
    {
        private TcpClient tcpClient;

        private StreamReader streamReader;
        private StreamWriter streamWriter;

        /// <summary>
        /// Indicates that data has been received through the connection
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Indicates that the TCP connection is completed
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Indicates that the TCP connection was closed
        /// </summary>
        public event EventHandler Disconnected;

        private readonly string host;
        private readonly int port;

        public TcpClientConnection(string host, int port = 6667)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (port <= 0)
            {
                throw new ArgumentException($"Port {port} is invalid.", nameof(port));
            }

            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Connects the client to an IRC server using the specified host and port number
        /// as an asynchronous operation
        /// </summary>
        /// <param name="host">The host of the IRC server</param>
        /// <param name="port">The port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync()
        {
            tcpClient?.Close();
            tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(host, port)
                .ConfigureAwait(false);

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());

            Connected?.Invoke(this, EventArgs.Empty);

            RunDataReceiver()
                .SafeFireAndForget(
                    continueOnCapturedContext: false,
                    onException: ex => Disconnected?.Invoke(this, EventArgs.Empty));
        }

        private async Task RunDataReceiver()
        {
            string line;

            while ((line = await streamReader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                DataReceived?.Invoke(this, new DataReceivedEventArgs(line));
            }

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sends raw data to the IRC server
        /// </summary>
        /// <param name="data">Data to be sent</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendAsync(string data)
        {
            if (!data.EndsWith(Constants.CrLf))
            {
                data += Constants.CrLf;
            }

            await streamWriter.WriteAsync(data)
                .ConfigureAwait(false);
            await streamWriter.FlushAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Disposes streams and the TcpClient
        /// </summary>
        public void Dispose()
        {
            streamReader?.Dispose();
            streamWriter?.Dispose();
            tcpClient.Dispose();
        }
    }
}
