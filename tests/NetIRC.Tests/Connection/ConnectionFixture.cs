using System.Net.Sockets;
using System.Net;
using System;

namespace NetIRC.Tests.Connection
{
    public class ConnectionFixture : IDisposable
    {
        public TcpListener TcpListener { get; }

        public ConnectionFixture()
        {
            TcpListener = new TcpListener(IPAddress.Any, 6667);
            TcpListener.Start();
        }

        public void Dispose()
        {
            TcpListener.Stop();
        }
    }
}
