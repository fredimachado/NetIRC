using Xunit;

namespace GravyIrc.Tests
{
    public class UserCollectionTests
    {
        [Fact]
        public void DoNotDuplicateUserWhenCallingGetUser()
        {
            var collection = new UserCollection();
            var user = new User("Test");
            
            collection.Add(user);
            var user2 = collection.GetUser("Test");

            Assert.Equal(1, collection.Count);
            Assert.Same(user, user2);
        }
    }
}
