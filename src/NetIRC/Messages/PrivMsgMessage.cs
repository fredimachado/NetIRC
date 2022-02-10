using NetIRC.Ctcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetIRC.Messages
{
    public class PrivMsgMessage : IRCMessage, IServerMessage, IClientMessage, ISplitClientMessage
    {
        public const int MaxMessageByteSize = 400;

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

        public IEnumerable<string[]> LineSplitTokens => BuildTokensFromMessageChunks();

        private IEnumerable<string[]> BuildTokensFromMessageChunks()
        {
            using var reader = new StringReader(Message);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var utf8Text = Encoding.UTF8.GetBytes(Message);

                var index = 0;
                var size = 0;
                var chunkStart = 0;
                while (index < utf8Text.Length)
                {
                    if (size >= MaxMessageByteSize)
                    {
                        var messageChunk = Encoding.UTF8.GetString(utf8Text.Skip(chunkStart).Take(size).ToArray());
                        yield return GetTokens(messageChunk);

                        // prepare for next chunk
                        chunkStart = index;
                        size = 0;
                    }

                    // 2-byte sequence, skip 1 character
                    if ((utf8Text[index] & 0xE0) == 0xC0)
                    {
                        index++;
                        size++;
                    }

                    // 3-byte sequence, skip 2 characters
                    if ((utf8Text[index] & 0xF0) == 0xE0)
                    {
                        index += 2;
                        size += 2;
                    }

                    // 4-byte sequence, skip 3 characters
                    if ((utf8Text[index] & 0xF8) == 0xF0)
                    {
                        index += 3;
                        size += 3;
                    }

                    // always skip at least 1 character and add 1 to size
                    index++;
                    size++;

                    // last chunk
                    if (index == utf8Text.Length)
                    {
                        var messageChunk = Encoding.UTF8.GetString(utf8Text.Skip(chunkStart).ToArray());
                        yield return GetTokens(messageChunk);
                    }
                }
            }
        }

        private string[] GetTokens(string message) => new[] { "PRIVMSG", To, message };
    }
}
