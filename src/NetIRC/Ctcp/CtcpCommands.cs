using NetIRC.Messages;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NetIRC.Ctcp
{
    /// <summary>
    /// Provides known CTCP command names and built-in CTCP reply handlers.
    /// </summary>
    public static class CtcpCommands
    {
        internal const string CtcpDelimiter = "\x01";

        /// <summary>
        /// CTCP ACTION command.
        /// </summary>
        public const string ACTION = nameof(ACTION);

        /// <summary>
        /// CTCP CLIENTINFO command.
        /// </summary>
        public const string CLIENTINFO = nameof(CLIENTINFO);

        /// <summary>
        /// CTCP ERRMSG command.
        /// </summary>
        public const string ERRMSG = nameof(ERRMSG);

        /// <summary>
        /// CTCP PING command.
        /// </summary>
        public const string PING = nameof(PING);

        /// <summary>
        /// CTCP TIME command.
        /// </summary>
        public const string TIME = nameof(TIME);

        /// <summary>
        /// CTCP VERSION command.
        /// </summary>
        public const string VERSION = nameof(VERSION);

        internal static Task HandleCtcp(Client client, CtcpEventArgs ctcp)
        {
            switch (ctcp.CtcpCommand.ToUpper())
            {
                case ACTION:
                case ERRMSG:
                    break;
                case CLIENTINFO:
                    return ClientInfoReply(client, ctcp.From);
                case PING:
                    return PingReply(client, ctcp.From, ctcp.CtcpMessage);
                case TIME:
                    return TimeReply(client, ctcp.From);
                case VERSION:
                    return VersionReply(client, ctcp.From);
            }

            return Task.CompletedTask;
        }

        private static Task ClientInfoReply(Client client, string target)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{CLIENTINFO} {ACTION} {CLIENTINFO} {PING} {TIME} {VERSION}"));
        }

        private static Task PingReply(Client client, string target, string message)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{PING} {message}"));
        }

        private static Task TimeReply(Client client, string target)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{TIME} {DateTime.Now:F}"));
        }

        private static Task VersionReply(Client client, string target)
        {
            var version = typeof(Client).Assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()
                .Version;
            return client.SendAsync(new CtcpReplyMessage(target, $"{VERSION} NetIRC v{version} - Simple cross-platform IRC Client Library (https://github.com/fredimachado/NetIRC)"));
        }
    }
}
