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
        private readonly TcpClient tcpClient = new TcpClient();

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

        private static string crlf = "\r\n";

        /// <summary>
        /// Connects the client to an IRC server using the specified address and port number
        /// as an asynchronous operation
        /// </summary>
        /// <param name="address">The address of the IRC server</param>
        /// <param name="port">The port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync(string address, int port)
        {
            await tcpClient.ConnectAsync(address, port)
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
            if (!data.EndsWith(crlf))
            {
                data += crlf;
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
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                streamReader?.Dispose();
                streamWriter?.Dispose();
                tcpClient.Dispose();
            }

            disposed = true;
        }

        ~TcpClientConnection()
        {
            Dispose(false);
        }
    }
}
