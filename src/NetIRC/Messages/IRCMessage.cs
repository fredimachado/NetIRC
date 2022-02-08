using System;
using System.Linq;
using System.Text;

namespace NetIRC.Messages
{
    public abstract class IRCMessage
    {
        /// <summary>
        /// When this IRC message was created
        /// </summary>
        public DateTime CreatedDate { get; } = DateTime.Now;

        public override string ToString()
        {
            return this switch
            {
                ISplitClientMessage clientMessage => BuildClientMessage(clientMessage),
                IClientMessage clientMessage => BuildClientMessage(clientMessage),
                _ => base.ToString(),
            };
        }

        private string BuildClientMessage(ISplitClientMessage clientMessage)
        {
            var sb = new StringBuilder(1024);

            foreach (var tokens in clientMessage.LineSplitTokens)
            {
                if (tokens.Length == 0)
                {
                    continue;
                }

                AppendTokens(sb, tokens);

                sb.Append(Constants.CrLf);
            }

            return sb.ToString().Trim();
        }

        private string BuildClientMessage(IClientMessage clientMessage)
        {
            var tokens = clientMessage.Tokens.ToArray();

            if (tokens.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(256);

            AppendTokens(sb, tokens);

            return sb.ToString().Trim();
        }

        private static void AppendTokens(StringBuilder sb, string[] tokens)
        {
            var lastIndex = tokens.Length - 1;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i == lastIndex && tokens[i].Contains(' '))
                {
                    sb.Append(':');
                }

                sb.Append(tokens[i]);

                if (i < lastIndex)
                {
                    sb.Append(' ');
                }
            }
        }
    }
}
