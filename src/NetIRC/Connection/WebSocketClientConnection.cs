using NetIRC.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    [ExcludeFromCodeCoverage]
    public class WebSocketClientConnection : IConnection
    {
        private readonly ClientWebSocket clientWebSocket = new ClientWebSocket();

        private CancellationTokenSource disposalTokenSource = new CancellationTokenSource();

        public event EventHandler<DataReceivedEventArgs> DataReceived;
        public event EventHandler Connected;
        public event EventHandler Disconnected;

        private readonly string address;

        public WebSocketClientConnection(string address)
        {
            this.address = address;
        }

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

        public async Task SendAsync(string data)
        {
            if (!data.EndsWith(Constants.CrLf))
            {
                data += Constants.CrLf;
            }

            var dataToSend = new ArraySegment<byte>(Encoding.ASCII.GetBytes(data));
            await clientWebSocket.SendAsync(dataToSend, WebSocketMessageType.Text, true, disposalTokenSource.Token)
                    .ConfigureAwait(false);
        }

        public void Dispose()
        {
            disposalTokenSource.Cancel();
            _ = clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
