﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetIRC.Messages
{
    public abstract class IRCMessage
    {
        public virtual IEnumerable<string> Tokens => Enumerable.Empty<string>();

        public override string ToString()
        {
            var tokens = Tokens.ToArray();

            if (tokens.Length == 0)
            {
                return string.Empty;
            }

            var lastIndex = tokens.Length - 1;

            var sb = new StringBuilder();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (i == lastIndex && tokens[i].Contains(" "))
                {
                    sb.Append(':');
                }

                sb.Append(tokens[i]);

                if (i < lastIndex)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}