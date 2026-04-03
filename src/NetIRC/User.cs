using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetIRC
{
    /// <summary>
    /// Represents an IRC user. Implements INotifyPropertyChanged
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of <see cref="User"/>.
        /// </summary>
        /// <param name="nick">Nickname used to identify the user.</param>
        public User(string nick)
        {
            if (string.IsNullOrWhiteSpace(nick)) throw new ArgumentException("Nick should not be empty.", nameof(nick));

            Nick = nick;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="User"/>.
        /// </summary>
        /// <param name="nick">Nickname used to identify the user.</param>
        /// <param name="realName">Real name advertised by the user.</param>
        public User(string nick, string realName) : this(nick)
        {
            RealName = realName;
        }

        private string nick;

        /// <summary>
        /// Gets or sets the user nickname.
        /// </summary>
        public string Nick
        {
            get { return nick; }
            set
            {
                if (nick != value)
                {
                    nick = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the real name value associated with the user.
        /// </summary>
        public string RealName { get; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
