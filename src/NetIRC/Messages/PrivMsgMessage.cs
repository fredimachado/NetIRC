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

                    // skip bytes that form a utf-8 character
                    int length = GetUtf8CharLength(utf8Text[index]);
                    index += length;
                    size += length;

                    // last chunk
                    if (index == utf8Text.Length)
                    {
                        var messageChunk = Encoding.UTF8.GetString(utf8Text.Skip(chunkStart).ToArray());
                        yield return GetTokens(messageChunk);
                    }
                }
            }

            int GetUtf8CharLength(byte b)
            {
                if (b < 0x80) return 1;
                else if ((b & 0xE0) == 0xC0) return 2;
                else if ((b & 0xF0) == 0xE0) return 3;
                else if ((b & 0xF8) == 0xF0) return 4;
                else if ((b & 0xfc) == 0xf8) return 5;
                else return 6;
            }
        }

        private string[] GetTokens(string message) => new[] { "PRIVMSG", To, message };
    }
}
