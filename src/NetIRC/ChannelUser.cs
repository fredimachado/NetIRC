using System;
using System.Collections;
using System.Collections.Generic;

namespace NetIRC
{
    /// <summary>
    /// Represents a user in a specific channel
    /// </summary>
    public class ChannelUser
    {
        /// <summary>
        /// Gets the backing user.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// Gets the channel status prefix (for example, @).
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Gets the user nickname.
        /// </summary>
        public string Nick => User.Nick;

        /// <summary>
        /// Initializes a new instance of <see cref="ChannelUser"/>.
        /// </summary>
        /// <param name="user">Underlying user.</param>
        /// <param name="status">Channel status prefix.</param>
        public ChannelUser(User user, string status)
        {
            User = user;
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ChannelUser"/> with no status.
        /// </summary>
        /// <param name="user">Underlying user.</param>
        public ChannelUser(User user)
            : this(user, string.Empty)
        {
        }

        /// <summary>
        /// Returns the displayed channel user value including status.
        /// </summary>
        /// <returns>String representation in the form <c>{status}{nick}</c>.</returns>
        public override string ToString() => Status + User.Nick;
    }

    /// <summary>
    /// Compares channel users by status rank and nickname.
    /// </summary>
    public class ChannelUserComparer : IComparer<ChannelUser>, IComparer
    {
        /// <summary>
        /// Compares two channel users.
        /// </summary>
        /// <param name="u1">First user.</param>
        /// <param name="u2">Second user.</param>
        /// <returns>A comparison result suitable for sorting.</returns>
        public int Compare(ChannelUser u1, ChannelUser u2)
        {
            if (!string.IsNullOrWhiteSpace(u1.Status) && !string.IsNullOrWhiteSpace(u2.Status))
            {
                var statuses = Channel.UserStatuses;
                var s1 = u1.Status[0];
                var s2 = u2.Status[0];
                if (Array.IndexOf(statuses, s1) < Array.IndexOf(statuses, s2))
                    return -1;
                if (Array.IndexOf(statuses, s1) > Array.IndexOf(statuses, s2))
                    return 1;
                return u1.Nick.CompareTo(u2.Nick);
            }

            if (!string.IsNullOrWhiteSpace(u1.Status))
                return -1;

            if (!string.IsNullOrWhiteSpace(u2.Status))
                return 1;

            return u1.Nick.CompareTo(u2.Nick);
        }

        /// <summary>
        /// Compares two objects as <see cref="ChannelUser"/> values.
        /// </summary>
        /// <param name="x">First object.</param>
        /// <param name="y">Second object.</param>
        /// <returns>A comparison result, or <c>0</c> if either object is not a <see cref="ChannelUser"/>.</returns>
        public int Compare(object x, object y)
        {
            if (x is not ChannelUser u1 || y is not ChannelUser u2)
                return 0;

            return Compare(u1, u2);
        }
    }
}
