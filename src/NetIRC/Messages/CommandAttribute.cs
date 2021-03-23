using System;

namespace NetIRC.Messages
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {
        public string Command { get; }

        public CommandAttribute(string command)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            Command = command.ToUpper();
        }
    }
}
