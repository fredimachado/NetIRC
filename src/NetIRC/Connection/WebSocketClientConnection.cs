using NetIRC.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    /// <summary>
    /// Represents a WebSocket connection to an IRC server.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WebSocketClientConnection : IConnection
    {
        private readonly ClientWebSocket clientWebSocket = new ClientWebSocket();

        private readonly CancellationTokenSource disposalTokenSource = new CancellationTokenSource();
        private bool disposed;

        /// <summary>
        /// Raised when data is received through the connection.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Raised when the WebSocket connection is completed.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Raised when the WebSocket connection is closed.
        /// </summary>
        public event EventHandler Disconnected;

        private readonly string address;

        /// <summary>
        /// Initializes a new WebSocket connection abstraction for an IRC server endpoint.
        /// </summary>
        /// <param name="address">Absolute WebSocket address (ws or wss).</param>
        public WebSocketClientConnection(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!Uri.TryCreate(address, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("Address must be a valid absolute URI.", nameof(address));
            }

            if (!string.Equals(uri.Scheme, "ws", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(uri.Scheme, "wss", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Address scheme must be ws or wss.", nameof(address));
            }

            this.address = address;
        }

        /// <summary>
        /// Connects to the configured WebSocket endpoint.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task ConnectAsync()
        {
            await clientWebSocket.ConnectAsync(new Uri(address), disposalTokenSource.Token)
                    .ConfigureAwait(false);

            while (clientWebSocket.State != WebSocketState.Open)
            {
                await Task.Delay(100)
                    .ConfigureAwait(false);
            }

            Connected?.Invoke(this, EventArgs.Empty);

            RunDataReceiver()
                .SafeFireAndForget(
                    continueOnCapturedContext: false,
                    onException: ex => Disconnected?.Invoke(this, EventArgs.Empty));
        }

        private async Task RunDataReceiver()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            while (!disposalTokenSource.IsCancellationRequested)
            {
                var received = await clientWebSocket.ReceiveAsync(buffer, disposalTokenSource.Token)
                    .ConfigureAwait(false);
                var receivedAsText = Encoding.ASCII.GetString(buffer.Array, 0, received.Count);

                DataReceived?.Invoke(this, new DataReceivedEventArgs(receivedAsText));
            }

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Sends raw IRC data through the WebSocket connection.
        /// </summary>
        /// <param name="data">Data to be sent.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public async Task SendAsync(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!data.EndsWith(Constants.CrLf))
            {
                data += Constants.CrLf;
            }

            var dataToSend = new ArraySegment<byte>(Encoding.ASCII.GetBytes(data));
            await clientWebSocket.SendAsync(dataToSend, WebSocketMessageType.Text, true, disposalTokenSource.Token)
                    .ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels active receives and closes the WebSocket connection.
        /// </summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            disposalTokenSource.Cancel();
            disposalTokenSource.Dispose();

            if (clientWebSocket.State == WebSocketState.Open || clientWebSocket.State == WebSocketState.CloseReceived)
            {
                _ = clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None)
                    .ConfigureAwait(false);
            }

            clientWebSocket.Dispose();
        }
    }
}
