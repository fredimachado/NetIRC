using Xunit;

namespace GravyIrc.Tests
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

            Assert.Equal(1, collection.Count);
            Assert.Same(query, query2);
        }
    }
}
