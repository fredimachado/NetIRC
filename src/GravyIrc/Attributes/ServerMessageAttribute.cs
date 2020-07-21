using System;

namespace GravyIrc.Attributes
{
    internal class ServerMessageAttribute : Attribute
    {
        public string Action { get; }

        public ServerMessageAttribute(string action)
        {
            Action = action;
        }
    }
}
