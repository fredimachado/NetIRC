﻿using System;
using System.Linq;

namespace NetIRC
{
    public class ParsedIRCMessage
    {
        public string Raw { get; }

        private IRCPrefix prefix;
        private string command;
        private string[] parameters;
        private string trailing;
        private IRCCommand ircCommand = IRCCommand.UNKNOWN;
        private IRCNumericReply numericReply = IRCNumericReply.UNKNOWN;

        public IRCPrefix Prefix => prefix;
        public string Command => command;
        public string[] Parameters => parameters;
        public string Trailing => trailing ?? string.Empty;
        public string Text => trailing ?? (parameters != null ? parameters[parameters.Length - 1] : string.Empty);
        public IRCCommand IRCCommand => ircCommand;
        public IRCNumericReply NumericReply => numericReply;
        public bool IsNumeric => numericReply != IRCNumericReply.UNKNOWN;

        public ParsedIRCMessage(string rawData)
        {
            Raw = rawData;
            Parse(rawData);
            ParseIRCEnums();
        }

        private void ParseIRCEnums()
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            if (IsNumericReply(command))
            {
                Enum.TryParse(command, out numericReply);
                if (IsNumericReply(numericReply.ToString()))
                {
                    numericReply = IRCNumericReply.UNKNOWN;
                }
            }
            else
            {
                Enum.TryParse(command, out ircCommand);
            }
        }

        private void Parse(string rawData)
        {
            var indexOfNextSpace = 0;

            if (RawDataHasPrefix)
            {
                indexOfNextSpace = rawData.IndexOf(' ');
                var prefixData = rawData.Substring(1, indexOfNextSpace - 1);
                prefix = new IRCPrefix(prefixData);
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

        private bool RawDataHasPrefix => Raw.StartsWith(":");

        private bool DataDoesNotContainSpaces(string data) => !data.Contains(" ");

        private bool IsNumericReply(string command) => command.Length == 3 && command.ToCharArray().All(char.IsDigit);

        public override string ToString()
        {
            var paramsDescription = parameters != null ? "{ " + string.Join(", ", parameters) + " }" : string.Empty;
            return $"Prefix: {prefix}, Command: {command}, Params: {paramsDescription}, Trailing: {trailing}";
        }
    }
}