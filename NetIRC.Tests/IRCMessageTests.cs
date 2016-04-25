using Xunit;

namespace NetIRC.Tests
{
    public class IRCMessageTests
    {
        [Fact]
        public void CanSetRawData()
        {
            var rawData = "raw";
            var ircMessage = new IRCMessage(rawData);

            Assert.Equal(rawData, ircMessage.Raw);
        }

        [Fact]
        public void CanParseThePrefix()
        {
            var prefix = "prefix";
            var ircMessage = new IRCMessage($":{prefix} ");

            Assert.Equal(prefix, ircMessage.Prefix);
        }

        [Fact]
        public void CanParseCommandWithSingleParameter()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var ircMessage = new IRCMessage($"{command} {parameter}");
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(parameter, ircMessage.Parameters[0]);
        }
    }
}
