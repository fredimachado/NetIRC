using System.Collections.Generic;

namespace GravyIrc.Messages
{
    public interface IClientMessage
    {
        IEnumerable<string> Tokens { get; }
    }
}
