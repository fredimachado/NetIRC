namespace NetIRC.Messages
{
    /// <summary>
    /// Exposes state for custom message handlers.
    /// </summary>
    public interface ICustomHandler
    {
        /// <summary>
        /// Gets whether the handler has already processed a message.
        /// </summary>
        bool Handled { get; }
    }
}
