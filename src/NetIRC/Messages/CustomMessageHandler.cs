namespace NetIRC.Messages
{
    public abstract class CustomMessageHandler<TServerMessage> : MessageHandler<TServerMessage>, ICustomHandler
        where TServerMessage : IServerMessage
    {
        public bool Handled { get; set; }
    }
}
