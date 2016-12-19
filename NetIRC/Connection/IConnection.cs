using System;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    public interface IConnection : IDisposable
    {
        Task ConnectAsync(string address, int port);
        Task SendAsync(string data);

        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler Connected;
        event EventHandler Disconnected;
    }
}
