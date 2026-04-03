namespace NetIRC
{
    /// <summary>
    /// Represents known non-numeric IRC command names.
    /// </summary>
    public enum IRCCommand
    {
        /// <summary>Unknown or unsupported command.</summary>
        UNKNOWN,
        /// <summary>NICK command.</summary>
        NICK,
        /// <summary>MODE command.</summary>
        MODE,
        /// <summary>QUIT command.</summary>
        QUIT,
        /// <summary>JOIN command.</summary>
        JOIN,
        /// <summary>PART command.</summary>
        PART,
        /// <summary>TOPIC command.</summary>
        TOPIC,
        /// <summary>INVITE command.</summary>
        INVITE,
        /// <summary>KICK command.</summary>
        KICK,
        /// <summary>PRIVMSG command.</summary>
        PRIVMSG,
        /// <summary>NOTICE command.</summary>
        NOTICE,
        /// <summary>PING command.</summary>
        PING,
        /// <summary>ERROR command.</summary>
        ERROR
    }
}
