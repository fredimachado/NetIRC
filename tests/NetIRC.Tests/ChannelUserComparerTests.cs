using System.Collections.Generic;
using Xunit;

namespace NetIRC.Tests
{
    public class ChannelUserComparerTests
    {
        [Fact]
        public void TestChannelUserSorting()
        {
            var channelUsers = new List<ChannelUser>
            {
                new ChannelUser(new User("Fredi")),
                new ChannelUser(new User("kikuch")),
                new ChannelUser(new User("Pe`tal"), "@"),
                new ChannelUser(new User("chain")),
                new ChannelUser(new User("ChanStat"), "%"),
                new ChannelUser(new User("Code"), "~"),
                new ChannelUser(new User("[420]"), "&"),
                new ChannelUser(new User("Zeus")),
                new ChannelUser(new User("Arch"), "%"),
                new ChannelUser(new User("Betty"), "~"),
                new ChannelUser(new User("NiK")),
                new ChannelUser(new User("JPGum"), "&"),
                new ChannelUser(new User("Andy"), "@"),
            };

            channelUsers.Sort(new ChannelUserComparer());

            Assert.Equal("~Betty", channelUsers[0].ToString());
            Assert.Equal("~Code", channelUsers[1].ToString());
            Assert.Equal("&[420]", channelUsers[2].ToString());
            Assert.Equal("&JPGum", channelUsers[3].ToString());
            Assert.Equal("@Andy", channelUsers[4].ToString());
            Assert.Equal("@Pe`tal", channelUsers[5].ToString());
            Assert.Equal("%Arch", channelUsers[6].ToString());
            Assert.Equal("%ChanStat", channelUsers[7].ToString());
            Assert.Equal("chain", channelUsers[8].ToString());
            Assert.Equal("Fredi", channelUsers[9].ToString());
            Assert.Equal("kikuch", channelUsers[10].ToString());
            Assert.Equal("NiK", channelUsers[11].ToString());
            Assert.Equal("Zeus", channelUsers[12].ToString());
        }

        [Fact]
        public void TestChannelUserComparer()
        {
            var u1 = new ChannelUser(new User("Fredi")) as object;
            var u2 = new ChannelUser(new User("kikuch")) as object;

            var comparer = new ChannelUserComparer();

            Assert.Equal(-1, comparer.Compare(u1, u2));
            Assert.Equal(1, comparer.Compare(u2, u1));
            Assert.Equal(0, comparer.Compare(u1, u1));

            Assert.Equal(0, comparer.Compare(u1, new object()));
            Assert.Equal(0, comparer.Compare(new object(), u2));
        }
    }
}
