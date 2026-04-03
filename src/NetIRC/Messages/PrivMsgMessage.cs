using NetIRC.Ctcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetIRC.Messages
{
    /// <summary>
    /// Represents a PRIVMSG message from server or client.
    /// </summary>
    public class PrivMsgMessage : IRCMessage, IServerMessage, IClientMessage, ISplitClientMessage
    {
        /// <summary>
        /// Maximum UTF-8 byte size per IRC message chunk.
        /// </summary>
        public const int MaxMessageByteSize = 400;

        /// <summary>
        /// Gets the sender nickname.
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Gets the parsed sender prefix.
        /// </summary>
        public IRCPrefix Prefix { get; }

        /// <summary>
        /// Gets the target nickname or channel.
        /// </summary>
        public string To { get; }

        /// <summary>
        /// Gets the message text.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets whether the target is a channel.
        /// </summary>
        public bool IsChannelMessage { get; }

        /// <summary>
        /// Gets whether the message contains CTCP framing.
        /// </summary>
        public bool IsCtcp { get; }

        /// <summary>
        /// Initializes a new instance from a parsed server message.
        /// </summary>
        /// <param name="parsedMessage">Parsed IRC message.</param>
        public PrivMsgMessage(ParsedIRCMessage parsedMessage)
        {
            From = parsedMessage.Prefix.From;
            Prefix = parsedMessage.Prefix;
            To = parsedMessage.Parameters[0];
            Message = parsedMessage.Trailing;

            IsChannelMessage = To[0] == '#';
            IsCtcp = Message.Contains(CtcpCommands.CtcpDelimiter);
        }

        /// <summary>
        /// Initializes a new instance for a client command.
        /// </summary>
        /// <param name="target">Target nickname or channel.</param>
        /// <param name="text">Message text.</param>
        public PrivMsgMessage(string target, string text)
        {
            To = target;
            Message = !text.Contains(" ") ? $":{text}" : text;
        }

        /// <summary>
        /// Gets command tokens for single-line serialization.
        /// </summary>
        public IEnumerable<string> Tokens => Enumerable.Empty<string>();

        /// <summary>
        /// Gets command tokens split into multiple lines when needed.
        /// </summary>
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
