using System;
using System.Collections.Generic;
using System.Linq;

namespace GravyIrc
{
    /// <summary>
    /// Represents a parsed IRC message, providing information about it
    /// </summary>
    public class ParsedIrcMessage
    {
        /// <summary>
        /// The raw message received from the server
        /// </summary>
        public string Raw { get; }

        /// <summary>
        /// The prefix of the message
        /// </summary>
        public IrcPrefix Prefix { get; private set; }

        /// <summary>
        /// The command received
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Provides all parameters received in the message
        /// </summary>
        public string[] Parameters { get; private set; }

        /// <summary>
        /// Represents the last parameters in the message
        /// </summary>
        public string Trailing => Parameters != null ? Parameters[Parameters.Length - 1] : string.Empty;

        /// <summary>
        /// An Enum representing the IRC command
        /// </summary>
        public IrcCommand IrcCommand { get; private set; }

        /// <summary>
        /// An Enum representing the IRC numeric reply
        /// </summary>
        public IrcNumericReply NumericReply { get; private set; }

        /// <summary>
        /// Provides you a way to quickly check if the message is a numeric reply
        /// </summary>
        public bool IsNumeric => NumericReply != IrcNumericReply.UNKNOWN;

        /// <summary>
        /// Initializes a new instance of ParsedIRCMessage, parsing the raw data
        /// </summary>
        /// <param name="rawData">Raw data to be parsed</param>
        public ParsedIrcMessage(string rawData)
        {
            Raw = rawData;
            Parse(rawData);
            ParseIrcEnums();
        }

        private void ParseIrcEnums()
        {
            if (string.IsNullOrEmpty(Command))
            {
                return;
            }

            if (IsNumericReply(Command))
            {
                if (Enum.TryParse(Command, out IrcNumericReply numericReply))
                {
                    NumericReply = numericReply;
                    if (IsNumericReply(NumericReply.ToString()))
                    {
                        NumericReply = IrcNumericReply.UNKNOWN;
                    }
                }
            }
            else
            {
                if (Enum.TryParse(Command, out IrcCommand ircCommand))
                {
                    IrcCommand = ircCommand;
                }
            }
        }

        private void Parse(string rawData)
        {
            var trailing = string.Empty;
            int indexOfNextSpace;

            if (RawDataHasPrefix)
            {
                indexOfNextSpace = rawData.IndexOf(' ');
                var prefixData = rawData.Substring(1, indexOfNextSpace - 1);
                Prefix = new IrcPrefix(prefixData);
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
                Command = rawData;

                if (!string.IsNullOrEmpty(trailing))
                {
                    this.Parameters = new[] { trailing };
                }

                return;
            }

            indexOfNextSpace = rawData.IndexOf(' ');
            Command = rawData.Remove(indexOfNextSpace);
            rawData = rawData.Substring(indexOfNextSpace + 1);

            var parameters = new List<string>(rawData.Split(' '));

            if (!string.IsNullOrEmpty(trailing))
            {
                parameters.Add(trailing);
            }

            this.Parameters = parameters.ToArray();
        }

        private bool RawDataHasPrefix => Raw.StartsWith(":");

        private bool DataDoesNotContainSpaces(string data) => !data.Contains(" ");

        private bool IsNumericReply(string command) => command.Length == 3 && command.ToCharArray().All(char.IsDigit);

        /// <summary>
        /// Returns a string that represents the parsed IRC message
        /// </summary>
        /// <returns>String that represents the parsed IRC message</returns>
        public override string ToString()
        {
            var paramsDescription = Parameters != null ? $"{{ {string.Join(", ", Parameters)} }}" : string.Empty;
            return $"Prefix: {Prefix}, Command: {Command}, Params: {paramsDescription}, Trailing: {Trailing}";
        }
    }
}
