using Xunit;

namespace NetIRC.Tests
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

            Assert.Single(collection);
            Assert.Same(user, user2);
        }
    }
}
