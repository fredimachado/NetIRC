using Xunit;

namespace NetIRC.Tests
{
    public class IRCPrefixTests
    {
        [Fact]
        public void CanParseSimplePrefix()
        {
            var prefix = "Nick123";
            var ircPrefix = new IRCPrefix(prefix);

            Assert.Equal(prefix, ircPrefix.From);
        }

        [Fact]
        public void CanParsePrefixWithHost()
        {
            var prefix = "Nick123@host.com";
            var ircPrefix = new IRCPrefix(prefix);

            Assert.Equal("Nick123", ircPrefix.From);
            Assert.Equal("host.com", ircPrefix.Host);
        }

        [Fact]
        public void CanParsePrefixWithUserAndHost()
        {
            var prefix = "Nick123!user@host.com";
            var ircPrefix = new IRCPrefix(prefix);

            Assert.Equal("Nick123", ircPrefix.From);
            Assert.Equal("user", ircPrefix.User);
            Assert.Equal("host.com", ircPrefix.Host);
        }

        [Fact]
        public void ToStringReturnsRawPrefix()
        {
            var prefix = "Nick123!user@host.com";
            var ircPrefix = new IRCPrefix(prefix);

            Assert.Equal(prefix, ircPrefix.ToString());
        }
    }
}
