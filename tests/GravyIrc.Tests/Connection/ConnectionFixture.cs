using System.Net.Sockets;
using System.Net;
using System;

namespace GravyIrc.Tests.Connection
{
    public class ConnectionFixture : IDisposable
    {
        public TcpListener TcpListener { get; }

        public ConnectionFixture()
        {
            TcpListener = new TcpListener(IPAddress.Loopback, 6669);
            TcpListener.Start();
        }

        public void Dispose()
        {
            TcpListener.Stop();
        }
    }
}
