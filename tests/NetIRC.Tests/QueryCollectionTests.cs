using Xunit;

namespace NetIRC.Tests
{
    public class QueryCollectionTests
    {
        [Fact]
        public void DoNotDuplicateQueryWhenCallingGetQuery()
        {
            var collection = new QueryCollection();
            var user = new User("Test");
            var query = new Query(user);
            
            collection.Add(query);
            var query2 = collection.GetQuery(user);

            Assert.Single(collection);
            Assert.Same(query, query2);
        }
    }
}
