using System.Collections.Generic;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a client message that can emit multiple wire lines.
    /// </summary>
    public interface ISplitClientMessage
    {
        /// <summary>
        /// Gets tokenized IRC commands split by output line.
        /// </summary>
        IEnumerable<string[]> LineSplitTokens { get; }
    }
}
