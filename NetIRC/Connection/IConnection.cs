using System;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    public interface IConnection : IDisposable
    {
        Task ConnectAsync(string address, int port);
        Task DisconnectAsync();
        Task SendAsync(string data);
        event EventHandler<DataReceivedEventArgs> DataReceived;
    }
}
