using Xunit;

namespace GravyIrc.Tests
{
    public class ParsedIRCMessageTests
    {
        [Fact]
        public void CanSetRawData()
        {
            var rawData = "raw";
            var ircMessage = new ParsedIrcMessage(rawData);

            Assert.Equal(rawData, ircMessage.Raw);
        }

        [Fact]
        public void CanParseThePrefix()
        {
            var prefix = "prefix";
            var ircMessage = new ParsedIrcMessage($":{prefix} ");

            Assert.Equal(prefix, ircMessage.Prefix.From);
        }

        [Fact]
        public void CanParseCommandWithSingleParameter()
        {
            var command = "PING";
            var parameter = "tolsun.oulu.fi";
            var ircMessage = new ParsedIrcMessage($"{command} {parameter}");
            Assert.Equal(command, ircMessage.Command);
            Assert.Equal(parameter, ircMessage.Parameters[0]);
        }

        [Fact]
        public void CanParseCommandWithTwoParameters()
        {
            var command = "MODE";
            var nick = "Angel";
            var mode = "+i";
            var ircMessage = new ParsedIrcMessage($"{command} {nick} {mode}");
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
            var ircMessage = new ParsedIrcMessage($":{prefix} {command} {target} :{text}");
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
            var ircMessage = new ParsedIrcMessage($":{prefix} {command} {target} :{text}");
            Assert.Equal($"Prefix: {prefix}, Command: {command}, Params: {{ {target}, {text} }}, Trailing: {text}", ircMessage.ToString());
        }

        [Fact]
        public void CanParseIRCCommand()
        {
            var data = "PING";
            var ircMessage = new ParsedIrcMessage(data);
            Assert.Equal(IrcCommand.PING, ircMessage.IrcCommand);
        }

        [Fact]
        public void SetUnknownIRCCommandIfNotFound()
        {
            var data = "PONG";
            var ircMessage = new ParsedIrcMessage(data);
            Assert.Equal(IrcCommand.UNKNOWN, ircMessage.IrcCommand);
        }

        [Fact]
        public void CanParseNumericReply()
        {
            var data = "001";
            var ircMessage = new ParsedIrcMessage(data);
            Assert.Equal(IrcNumericReply.RPL_WELCOME, ircMessage.NumericReply);
        }

        [Fact]
        public void SetUnknownNumericReplyIfNotFound()
        {
            var data = "900";
            var ircMessage = new ParsedIrcMessage(data);
            Assert.Equal(IrcNumericReply.UNKNOWN, ircMessage.NumericReply);
        }

        [Fact]
        public void SetIsNumericProperty()
        {
            var data = "001";
            var ircMessage = new ParsedIrcMessage(data);
            Assert.Equal(true, ircMessage.IsNumeric);
        }
    }
}
