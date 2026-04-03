using NetIRC.Connection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests.Connection
{
    public class WebSocketClientConnectionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void WhenCreatingConnectionInstance_ShouldNotProvideNullOrEmptyAddress(string address)
        {
            Assert.Throws<ArgumentNullException>(() => new WebSocketClientConnection(address));
        }

        [Theory]
        [InlineData("not-a-uri")]
        [InlineData("localhost:1234")]
        public void WhenCreatingConnectionInstance_ShouldProvideValidAbsoluteAddress(string address)
        {
            Assert.Throws<ArgumentException>(() => new WebSocketClientConnection(address));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task WhenSendingInvalidData_ShouldThrowArgumentNullException(string data)
        {
            using var connection = new WebSocketClientConnection("ws://localhost:12345");
            await Assert.ThrowsAsync<ArgumentNullException>(() => connection.SendAsync(data));
        }
    }
}
