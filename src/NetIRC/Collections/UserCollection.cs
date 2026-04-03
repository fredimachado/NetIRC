using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// An observable collection that represents all users the client knows about
    /// </summary>
    public class UserCollection : ObservableCollection<User>
    {
        /// <summary>
        /// Gets an existing user by nickname, or creates one if it does not exist.
        /// </summary>
        /// <param name="nick">Nickname to search for.</param>
        /// <returns>The existing or newly created <see cref="User"/>.</returns>
        public User GetUser(string nick)
        {
            var user = Items.FirstOrDefault(u => string.Equals(u.Nick, nick, StringComparison.InvariantCultureIgnoreCase));

            if (user is null)
            {
                user = new User(nick);
                Client.DispatcherInvoker.Invoke(() => Add(user));
            }

            return user;
        }
    }
}
