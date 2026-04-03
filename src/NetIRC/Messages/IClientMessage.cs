using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a message that can be serialized and sent by the client.
    /// </summary>
    public interface IClientMessage
    {
        /// <summary>
        /// Gets the tokenized IRC command parts.
        /// </summary>
        IEnumerable<string> Tokens { get; }
    }
}
