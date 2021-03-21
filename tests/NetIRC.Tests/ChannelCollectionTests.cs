using Xunit;

namespace NetIRC.Tests
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

            Assert.Single(collection);
            Assert.Same(channel, channel2);
        }
    }
}
