namespace NetIRC.Messages
{
    public class RplWelcomeMessage : IRCMessage, IServerMessage
    {
        public string Text { get; }

        public RplWelcomeMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
