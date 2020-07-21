using System;

namespace GravyIrc.Attributes
{
    internal class ServerMessageAttribute : Attribute
    {
        public string Command { get; }

        public ServerMessageAttribute(string action)
        {
            Command = action;
        }
    }
}
