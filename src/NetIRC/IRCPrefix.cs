namespace NetIRC
{
    /// <summary>
    /// Represents a parsed IRC prefix section (for example: nick!user@host).
    /// </summary>
    public class IRCPrefix
    {
        /// <summary>
        /// Gets the original unparsed prefix value.
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// Gets the nickname or source part of the prefix.
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Gets the user part of the prefix when present.
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Gets the host part of the prefix when present.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="IRCPrefix"/> from raw prefix data.
        /// </summary>
        /// <param name="prefixData">Raw prefix text provided by the IRC message.</param>
        public IRCPrefix(string prefixData)
        {
            Raw = prefixData;
            From = prefixData;

            if (prefixData.Contains("@"))
            {
                var splitedPrefix = prefixData.Split('@');
                From = splitedPrefix[0];
                Host = splitedPrefix[1];
            }

            if (From.Contains("!"))
            {
                var splittedFrom = From.Split('!');
                From = splittedFrom[0];
                User = splittedFrom[1];
            }
        }

        /// <summary>
        /// Returns the raw prefix value.
        /// </summary>
        /// <returns>The original prefix text.</returns>
        public override string ToString()
        {
            return Raw;
        }
    }
}
