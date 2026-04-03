using System.Threading.Tasks;

namespace NetIRC.Messages
{
    /// <summary>
    /// Base abstraction for strongly typed message handlers.
    /// </summary>
    /// <typeparam name="TServerMessage">Type of server message handled.</typeparam>
    public abstract class MessageHandler<TServerMessage> : IMessageHandler<TServerMessage>
        where TServerMessage : IServerMessage
    {
        /// <summary>
        /// Gets the current message being handled.
        /// </summary>
        public TServerMessage Message { get; internal set; }

        /// <summary>
        /// Handles a parsed server message.
        /// </summary>
        /// <param name="serverMessage">Server message instance.</param>
        /// <param name="client">Client receiving the message.</param>
        /// <returns>A task that completes when handling finishes.</returns>
        public abstract Task HandleAsync(TServerMessage serverMessage, Client client);
    }
}
