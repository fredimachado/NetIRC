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

        [Fact]
        public void CanParseCommandWithTwoParameters()
        {
            var command = "MODE";
            var nick = "Angel";
            var mode = "+i";
            var ircMessage = new IRCMessage($"{command} {nick} {mode}");
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(nick, ircMessage.Parameters[0]);
            Assert.Equal(mode, ircMessage.Parameters[1]);
        }

        [Fact]
        public void CanParseCommandWithTrailing()
        {
            var prefix = "Angel!wings@irc.org";
            var command = "PRIVMSG";
            var target = "Wiz";
            var text = "Are you receiving this message ?";
            var ircMessage = new IRCMessage(string.Format(":{0} {1} {2} :{3}", prefix, command, target, text));
            Assert.Equal(prefix, ircMessage.Prefix);
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(target, ircMessage.Parameters[0]);
            Assert.Equal(text, ircMessage.Parameters[1]);
        }
    }
}
