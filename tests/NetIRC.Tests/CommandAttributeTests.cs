using NetIRC.Messages;
using System;
using Xunit;

namespace NetIRC.Tests
{
    public class CommandAttributeTests
    {
        [Fact]
        public void TestInvalidCommandAttribute()
        {
            Assert.Throws<ArgumentNullException>(() => new CommandAttribute(null));
        }

        [Fact]
        public void CommandShouldBeUpperCase()
        {
            var command = new CommandAttribute("notice");

            Assert.Equal("NOTICE", command.Command);
        }
    }
}
