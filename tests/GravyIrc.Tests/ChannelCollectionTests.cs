using Xunit;

namespace GravyIrc.Tests
{
    public class ChannelCollectionTests
    {
        [Fact]
        public void DoNotDuplicateChannelWhenCallingGetChannel()
        {
            var collection = new ChannelCollection();
            var channel = new Channel("#test");
            
            collection.Add(channel);
            var channel2 = collection.GetChannel("#test");

            Assert.Equal(1, collection.Count);
            Assert.Same(channel, channel2);
        }
    }
}
