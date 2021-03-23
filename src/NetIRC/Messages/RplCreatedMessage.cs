namespace NetIRC.Messages
{
    public class RplCreatedMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplCreatedMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
