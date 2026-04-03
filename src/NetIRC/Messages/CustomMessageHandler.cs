namespace NetIRC.Messages
{
    /// <summary>
    /// Base class for custom handlers that process server messages.
    /// </summary>
    /// <typeparam name="TServerMessage">Type of server message handled.</typeparam>
    public abstract class CustomMessageHandler<TServerMessage> : MessageHandler<TServerMessage>, ICustomHandler
        where TServerMessage : IServerMessage
    {
        /// <summary>
        /// Gets or sets whether this handler has already processed a message.
        /// </summary>
        public bool Handled { get; set; }
    }
}
