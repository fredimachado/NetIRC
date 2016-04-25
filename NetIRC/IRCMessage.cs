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

        public string Prefix => prefix;
        public string Command => command;
        public string[] Parameters => parameters;

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

            if (DataDoesNotContainSpaces(rawData))
            {
                command = rawData;
                return;
            }

            indexOfNextSpace = rawData.IndexOf(' ');
            command = rawData.Remove(indexOfNextSpace);
            rawData = rawData.Substring(indexOfNextSpace + 1);

            ParseParameters(rawData);
        }

        private void ParseParameters(string rawData)
        {
            var indexOfNextSpace = 0;
            var parsedParameters = new List<string>();

            while (!string.IsNullOrEmpty(rawData))
            {
                if (DataStartsWithColon(rawData))
                {
                    parsedParameters.Add(rawData.Substring(1));
                    break;
                }

                if (DataDoesNotContainSpaces(rawData))
                {
                    parsedParameters.Add(rawData);
                    break;
                }

                indexOfNextSpace = rawData.IndexOf(' ');

                parsedParameters.Add(rawData.Remove(indexOfNextSpace));
                rawData = rawData.Substring(indexOfNextSpace + 1);
            }

            parameters = parsedParameters.ToArray();
        }

        public bool RawDataHasPrefix => DataStartsWithColon(Raw);

        public bool DataStartsWithColon(string data) => data.StartsWith(":");

        public bool DataDoesNotContainSpaces(string data) => !data.Contains(" ");
    }
}
