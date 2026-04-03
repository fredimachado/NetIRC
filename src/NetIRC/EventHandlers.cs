namespace NetIRC
{
    /// <summary>
    /// Represents a callback invoked when raw IRC data is received.
    /// </summary>
    /// <param name="client">The client instance that received the data.</param>
    /// <param name="rawData">The raw data payload from the server.</param>
    public delegate void IRCRawDataHandler(Client client, string rawData);

    /// <summary>
    /// Represents a callback invoked when a raw IRC message is parsed.
    /// </summary>
    /// <param name="client">The client instance that parsed the message.</param>
    /// <param name="ircMessage">The parsed IRC message.</param>
    public delegate void ParsedIRCMessageHandler(Client client, ParsedIRCMessage ircMessage);
}
