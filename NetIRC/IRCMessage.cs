using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIRC
{
    public class IRCMessage
    {
        public string Raw { get; }

        private string prefix;

        public string Prefix => prefix;

        public IRCMessage(string rawData)
        {
            Raw = rawData;
            Parse(rawData);
        }

        private void Parse(string rawData)
        {
            var indexOfNextSpace = 0;

            if (RawDataHasPrefix)
            {
                indexOfNextSpace = rawData.IndexOf(' ');
                prefix = rawData.Substring(1, indexOfNextSpace - 1);
            }
        }

        public bool RawDataHasPrefix => Raw.StartsWith(":");
    }
}
