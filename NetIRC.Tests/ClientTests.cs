using Moq;
using NetIRC.Connection;
using System;
using Xunit;

namespace NetIRC.Tests
{
    public class ClientTests
    {
        [Fact]
        public void TriggersRawDataReceived()
        {
            var raw = "PING";
            var rawReceived = string.Empty;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            client.OnRawDataReceived += (c, d) => rawReceived = d;

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            Assert.Equal(raw, rawReceived);
        }

        [Fact]
        public void RespondsToPingMessage()
        {
            var data = "xyz.com";
            var raw = $"PING :{data}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(mockConnection.Object);

            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));

            mockConnection.Verify(c => c.SendAsync($"PONG :{data}"), Times.Once());
        }
    }
}
