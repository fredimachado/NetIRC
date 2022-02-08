using NetIRC.Ctcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetIRC.Messages
{
    public class PrivMsgMessage : IRCMessage, IServerMessage, IClientMessage, ISplitClientMessage
    {
        private const int MaxMessageLength = 400;

        public string From { get; }
        public IRCPrefix Prefix { get; }
        public string To { get; }
        public string Message { get; }
        public bool IsChannelMessage { get; }
        public bool IsCtcp { get; }

        public PrivMsgMessage(ParsedIRCMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Prefix = parsedMessage.Prefix;
            To = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;

            IsChannelMessage = To[0] == '#';
            IsCtcp = Message.Contains(CtcpCommands.CtcpDelimiter);
        }

        public PrivMsgMessage(string target, string text)
        {
            To = target;
            Message = !text.Contains(" ") ? $":{text}" : text;
        }

        public IEnumerable<string> Tokens => Enumerable.Empty<string>();

        public IEnumerable<string[]> LineSplitTokens => BuildTokens();

        private IEnumerable<string[]> BuildTokens()
        {
            if (Message.Length <= MaxMessageLength)
            {
                yield return GetTokens(Message);
                yield break;
            }

            using (var reader = new StringReader(Message))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    if (line.Length <= MaxMessageLength)
                    {
                        yield return GetTokens(line);
                    }
                    else
                    {
                        for (int i = 0; i < line.Length; i += MaxMessageLength)
                        {
                            var message = line.Substring(i, Math.Min(MaxMessageLength, line.Length - i));
                            yield return GetTokens(message);
                        }
                    }
                }
            }
        }

        private string[] GetTokens(string message) => new[] { "PRIVMSG", To, message };
    }
}
