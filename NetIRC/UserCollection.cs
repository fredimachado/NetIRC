using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    public class UserCollection : ObservableCollection<User>
    {
        public User GetUser(string nick)
        {
            var user = Items.FirstOrDefault(u => u.Nick == nick);

            if (user == null)
            {
                user = new User(nick);
                Add(user);
            }

            return user;
        }
    }
}
