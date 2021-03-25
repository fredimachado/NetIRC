namespace NetIRC.Messages
{
    public class KickMessage : IRCMessage, IServerMessage
    {
        public string KickedBy { get; }
        public string Channel { get; }
        public string Nick { get; }
        public string Comment { get; set; }

        public KickMessage(ParsedIRCMessage parsedMessage)
        {
            KickedBy = parsedMessage.Prefix.From;
            Channel = parsedMessage.Parameters[0];
            Nick = parsedMessage.Parameters[1];
            Comment = parsedMessage.Trailing;
        }
    }
}
