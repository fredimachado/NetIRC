using NetIRC.Messages;

namespace NetIRC.ConsoleCli
{
    // Message of the day is not defined in NetIRC, so we create our own
    // It must implement IServerMessage and the constructor needs to have a single parameter of type ParsedIRCMessage
    public class MotdStartMessage : IServerMessage
    {
        public string Text { get; }

        public MotdStartMessage(ParsedIRCMessage parsedMessage)
        {
            Text = parsedMessage.Trailing;
        }
    }
}
