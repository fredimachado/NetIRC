﻿using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// An observable collection that represents all queries the client has
    /// </summary>
    public class QueryCollection : ObservableCollection<Query>
    {
        public Query GetQuery(User user)
        {
            var query = Items.FirstOrDefault(q => q.User.Nick == user.Nick);

            if (query is null)
            {
                query = new Query(user);
                Client.DispatcherInvoker.Invoke(() => Add(query));
            }

            return query;
        }
    }
}
