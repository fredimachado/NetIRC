using System;
using System.Threading.Tasks;

namespace NetIRC.Connection
{
    /// <summary>
    /// Represents an interface for a connection
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// Connects to an IRC server.
        /// </summary>
        /// <returns>The task that represents the asynchronous connect operation.</returns>
        Task ConnectAsync();

        /// <summary>
        /// Sends raw IRC data through the connection.
        /// </summary>
        /// <param name="data">Raw IRC payload.</param>
        /// <returns>The task that represents the asynchronous send operation.</returns>
        Task SendAsync(string data);

        /// <summary>
        /// Raised when data is received from the server.
        /// </summary>
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Raised when the connection has been established.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Raised when the connection has been closed.
        /// </summary>
        event EventHandler Disconnected;
    }
}
