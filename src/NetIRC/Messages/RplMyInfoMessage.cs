namespace NetIRC.Messages
{
    public class RplMyInfoMessage : IRCMessage, IServerMessage
    {
        public string[] Parameters { get; }

        public RplMyInfoMessage(ParsedIRCMessage parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
        }
    }
}
