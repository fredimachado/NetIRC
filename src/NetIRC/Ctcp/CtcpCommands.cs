using NetIRC.Messages;
using System;
using System.Threading.Tasks;

namespace NetIRC.Ctcp
{
    public static class CtcpCommands
    {
        internal const string CtcpDelimiter = "\x01";

        public const string ACTION = nameof(ACTION);

        public const string CLIENTINFO = nameof(CLIENTINFO);

        public const string ERRMSG = nameof(ERRMSG);

        public const string PING = nameof(PING);

        public const string TIME = nameof(TIME);

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
            return client.SendAsync(new CtcpReplyMessage(target, ($"{CLIENTINFO} {ACTION} {CLIENTINFO} {PING} {TIME} {VERSION}")));
        }

        private static Task PingReply(Client client, string target, string message)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{PING} {message}"));
        }

        private static Task TimeReply(Client client, string target)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{TIME} {DateTime.Now.ToString("F")}"));
        }

        private static Task VersionReply(Client client, string target)
        {
            return client.SendAsync(new CtcpReplyMessage(target, $"{VERSION} NetIRC v{typeof(Client).Assembly.GetName().Version} - Open source IRC Client Library (https://github.com/fredimachado/NetIRC)"));
        }
    }
}
