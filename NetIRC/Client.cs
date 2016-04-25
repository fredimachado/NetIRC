﻿using NetIRC.Connection;
using System;
using System.Threading.Tasks;

namespace NetIRC
{
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        public event IRCRawDataHandler OnRawDataReceived;

        public Client(IConnection connection)
        {
            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;
        }

        private async void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            if (rawData.StartsWith("PING :"))
            {
                await connection.SendAsync("PONG" + rawData.Substring(4));
            }
        }

        public async Task ConnectAsync(string host, int port, string nick, string user)
        {
            await connection.ConnectAsync(host, port);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
