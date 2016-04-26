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
        private string command;
        private string[] parameters;
        private string trailing = string.Empty;

        public string Prefix => prefix;
        public string Command => command;
        public string[] Parameters => parameters;
        public string Trailing => trailing;

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
                rawData = rawData.Substring(indexOfNextSpace + 1);
            }

            var indexOfTrailingStart = rawData.IndexOf(" :");
            if (indexOfTrailingStart > -1)
            {
                trailing = rawData.Substring(indexOfTrailingStart + 2);
                rawData = rawData.Substring(0, indexOfTrailingStart);
            }

            if (DataDoesNotContainSpaces(rawData))
            {
                command = rawData;
                return;
            }

            indexOfNextSpace = rawData.IndexOf(' ');
            command = rawData.Remove(indexOfNextSpace);
            rawData = rawData.Substring(indexOfNextSpace + 1);

            parameters = rawData.Split(' ');
        }

        public bool RawDataHasPrefix => Raw.StartsWith(":");

        public bool DataDoesNotContainSpaces(string data) => !data.Contains(" ");
    }
}
