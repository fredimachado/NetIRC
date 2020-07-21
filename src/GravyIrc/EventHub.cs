using GravyIrc.Messages;
using System;

namespace GravyIrc
{
    public class EventHub
    {
        private readonly Client client;

        internal EventHub(Client client)
        {
            this.client = client;
        }

        /// <summary>
        /// Indicates that we are properly registered on the server
        /// It happens when the server sends Replies 001 to 004 to a user upon successful registration
        /// </summary>
        public event EventHandler RegistrationCompleted;
        internal void OnRegistrationCompleted()
        {
            RegistrationCompleted?.Invoke(client, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that we received a PING message from the server
        /// The client automatically sends a PONG message response
        /// </summary>
        public event IrcMessageEventHandler<PingMessage> Ping;
        internal void OnPing(IrcMessageEventArgs<PingMessage> e)
        {
            Ping?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a PRIVMSG message and provides you a PrivMsgMessage object
        /// </summary>
        public event IrcMessageEventHandler<PrivMsgMessage> PrivMsg;
        internal void OnPrivMsg(IrcMessageEventArgs<PrivMsgMessage> e)
        {
            PrivMsg?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a NOTICE message and provides you a NoticeMessage object
        /// </summary>
        public event IrcMessageEventHandler<NoticeMessage> Notice;
        internal void OnNotice(IrcMessageEventArgs<NoticeMessage> e)
        {
            Notice?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer changed his Nickname
        /// </summary>
        public event IrcMessageEventHandler<NickMessage> Nick;
        internal void OnNick(IrcMessageEventArgs<NickMessage> e)
        {
            Nick?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 001 (RPL_WELCOME) numeric reply message
        /// </summary>
        public event IrcMessageEventHandler<RplWelcomeMessage> RplWelcome;
        internal void OnRplWelcome(IrcMessageEventArgs<RplWelcomeMessage> e)
        {
            RplWelcome?.Invoke(client, e);
            OnRegistrationCompleted();
        }

        /// <summary>
        /// Indicates that we received a 002 (RPL_YOURHOST) numeric reply message
        /// </summary>
        public event IrcMessageEventHandler<RplYourHostMessage> RplYourHost;
        internal void OnRplYourHost(IrcMessageEventArgs<RplYourHostMessage> e)
        {
            RplYourHost?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 003 (RPL_CREATED) numeric reply message
        /// </summary>
        public event IrcMessageEventHandler<RplCreatedMessage> RplCreated;
        internal void OnRplCreated(IrcMessageEventArgs<RplCreatedMessage> e)
        {
            RplCreated?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 004 (RPL_MYINFO) numeric reply message
        /// </summary>
        public event IrcMessageEventHandler<RplMyInfoMessage> RplMyInfo;
        internal void OnRplMyInfo(IrcMessageEventArgs<RplMyInfoMessage> e)
        {
            RplMyInfo?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 005 (RPL_ISUPPORT) numeric reply message
        /// </summary>
        public event IrcMessageEventHandler<RplISupportMessage> RplISupport;
        internal void OnRplISupport(IrcMessageEventArgs<RplISupportMessage> e)
        {
            RplISupport?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has joined a channel
        /// </summary>
        public event IrcMessageEventHandler<JoinMessage> Join;
        internal void OnJoin(IrcMessageEventArgs<JoinMessage> e)
        {
            Join?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has left a channel
        /// </summary>
        public event IrcMessageEventHandler<PartMessage> Part;
        internal void OnPart(IrcMessageEventArgs<PartMessage> e)
        {
            Part?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates we received a 353 (RPL_NAMREPLY) numeric reply
        /// which contains the list of all users in a channel
        /// </summary>
        public event IrcMessageEventHandler<RplNamReplyMessage> RplNamReply;
        internal void OnRplNamReply(IrcMessageEventArgs<RplNamReplyMessage> e)
        {
            RplNamReply?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has quit the server
        /// </summary>
        public event IrcMessageEventHandler<QuitMessage> Quit;
        internal void OnQuit(IrcMessageEventArgs<QuitMessage> e)
        {
            Quit?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that the bot has been kicked from a channel
        /// </summary>
        public event IrcMessageEventHandler<KickMessage> Kick;
        internal void OnKick(IrcMessageEventArgs<KickMessage> e)
        {
            Kick?.Invoke(client, e);
        }
    }
}
