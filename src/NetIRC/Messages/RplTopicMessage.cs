namespace NetIRC.Messages
{
    public class RplTopicMessage : IRCMessage, IServerMessage
    {
        public string Channel { get; }
        public string Topic { get; }

        public RplTopicMessage(ParsedIRCMessage parsedMessage)
        {
            Channel = parsedMessage.Parameters[2];
            Topic = parsedMessage.Trailing;
        }
    }
}
