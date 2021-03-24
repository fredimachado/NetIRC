namespace NetIRC
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void ParsedIRCMessageHandler(Client client, ParsedIRCMessage ircMessage);
}
