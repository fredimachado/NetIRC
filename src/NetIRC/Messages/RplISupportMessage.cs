namespace NetIRC.Messages
{
    public class RplISupportMessage : IRCMessage, IServerMessage
    {
        public string[] Parameters { get; }
        public string Text { get; }

        public RplISupportMessage(ParsedIRCMessage parsedMessage)
        {
            Parameters = parsedMessage.Parameters;
            Text = parsedMessage.Trailing;
        }
    }
}
