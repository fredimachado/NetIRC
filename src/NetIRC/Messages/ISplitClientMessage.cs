using System.Collections.Generic;

namespace NetIRC.Messages
{
    public interface ISplitClientMessage
    {
        IEnumerable<string[]> LineSplitTokens { get; }
    }
}
