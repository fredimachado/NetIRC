using Xunit;

namespace NetIRC.Tests
{
    public class ParsedIRCMessageTests
    {
        [Fact]
        public void CanSetRawData()
        {
            var rawData = "raw";
            var ircMessage = new ParsedIRCMessage(rawData);

            Assert.Equal(rawData, ircMessage.Raw);
        }

        [Fact]
        public void CanParseThePrefix()
        {
            var prefix = "prefix";
            var ircMessage = new ParsedIRCMessage($":{prefix} ");

            Assert.Equal(prefix, ircMessage.Prefix.From);
        }

        [Fact]
        public void CanParseCommandWithSingleParameter()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var ircMessage = new ParsedIRCMessage($"{command} {parameter}");
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(parameter, ircMessage.Parameters[0]);
        }

        [Fact]
        public void CanParseCommandWithTwoParameters()
        {
            var command = "MODE";
            var nick = "Angel";
            var mode = "+i";
            var ircMessage = new ParsedIRCMessage($"{command} {nick} {mode}");
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
            var ircMessage = new ParsedIRCMessage($":{prefix} {command} {target} :{text}");
            Assert.Equal(prefix, ircMessage.Prefix.Raw);
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(target, ircMessage.Parameters[0]);
            Assert.Equal(text, ircMessage.Trailing);
        }

        [Fact]
        public void GoodToStringOverride()
        {
            var prefix = "Angel!wings@irc.org";
            var command = "PRIVMSG";
            var target = "Wiz";
            var text = "Are you receiving this message ?";
            var ircMessage = new ParsedIRCMessage($":{prefix} {command} {target} :{text}");
            Assert.Equal($"Prefix: {prefix}, Command: {command}, Params: {{ {target}, {text} }}, Trailing: {text}", ircMessage.ToString());
        }

        [Fact]
        public void CanParseIRCCommand()
        {
            var data = "PING";
            var ircMessage = new ParsedIRCMessage(data);
            Assert.Equal(IRCCommand.PING, ircMessage.IRCCommand);
        }

        [Fact]
        public void SetUnknownIRCCommandIfNotFound()
        {
            var data = "PONG";
            var ircMessage = new ParsedIRCMessage(data);
            Assert.Equal(IRCCommand.UNKNOWN, ircMessage.IRCCommand);
        }

        [Fact]
        public void CanParseNumericReply()
        {
            var data = "001";
            var ircMessage = new ParsedIRCMessage(data);
            Assert.Equal(IRCNumericReply.RPL_WELCOME, ircMessage.NumericReply);
        }

        [Fact]
        public void SetUnknownNumericReplyIfNotFound()
        {
            var data = "900";
            var ircMessage = new ParsedIRCMessage(data);
            Assert.Equal(IRCNumericReply.UNKNOWN, ircMessage.NumericReply);
        }

        [Fact]
        public void SetIsNumericProperty()
        {
            var data = "001";
            var ircMessage = new ParsedIRCMessage(data);
            Assert.True(ircMessage.IsNumeric);
        }
    }
}
