namespace NetIRC
{
    public delegate void IRCRawDataHandler(Client client, string rawData);
    public delegate void IRCMessageHandler(Client client, IRCMessage ircMessage);
}
