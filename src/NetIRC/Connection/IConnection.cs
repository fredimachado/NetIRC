using System;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    /// <summary>
    /// Represents an interface for a connection
    /// </summary>
    public interface IConnection : IDisposable
    {
        Task ConnectAsync();
        Task SendAsync(string data);

        event EventHandler<DataReceivedEventArgs> DataReceived;
        event EventHandler Connected;
        event EventHandler Disconnected;
    }
}
