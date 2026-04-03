using System.Collections.ObjectModel;

namespace NetIRC
{
    /// <summary>
    /// Represents a query (private chat).
    /// </summary>
    public class Query
    {
        /// <summary>
        /// Gets the user represented by this query.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets the query nickname.
        /// </summary>
        public string Nick => User.Nick;

        /// <summary>
        /// Gets the list of private messages for this query.
        /// </summary>
        public ObservableCollection<QueryMessage> Messages { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="Query"/>.
        /// </summary>
        /// <param name="user">Remote user for the private conversation.</param>
        public Query(User user)
        {
            User = user;
            Messages = new ObservableCollection<QueryMessage>();
        }
    }
}
