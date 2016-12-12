using NetIRC.Messages;
using System;

namespace NetIRC
{
    public class EventHub
    {
        private readonly Client client;

        internal EventHub(Client client)
        {
            this.client = client;
        }

        /// <summary>
        /// The server sends Replies 001 to 004 to a user upon successful registration.
        /// </summary>
        public event EventHandler RegistrationCompleted;
        internal void OnRegistrationCompleted()
        {
            RegistrationCompleted?.Invoke(client, EventArgs.Empty);
        }

        public event IRCMessageEventHandler<PingMessage> Ping;
        internal void OnPing(IRCMessageEventArgs<PingMessage> e)
        {
            Ping?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<PrivMsgMessage> PrivMsg;
        internal void OnPrivMsg(IRCMessageEventArgs<PrivMsgMessage> e)
        {
            PrivMsg?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<NoticeMessage> Notice;
        internal void OnNotice(IRCMessageEventArgs<NoticeMessage> e)
        {
            Notice?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<NickMessage> Nick;
        internal void OnNick(IRCMessageEventArgs<NickMessage> e)
        {
            Nick?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<RplWelcomeMessage> RplWelcome;
        internal void OnRplWelcome(IRCMessageEventArgs<RplWelcomeMessage> e)
        {
            RplWelcome?.Invoke(client, e);
            OnRegistrationCompleted();
        }

        public event IRCMessageEventHandler<RplYourHostMessage> RplYourHost;
        internal void OnRplYourHost(IRCMessageEventArgs<RplYourHostMessage> e)
        {
            RplYourHost?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<RplCreatedMessage> RplCreated;
        internal void OnRplCreated(IRCMessageEventArgs<RplCreatedMessage> e)
        {
            RplCreated?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<RplMyInfoMessage> RplMyInfo;
        internal void OnRplMyInfo(IRCMessageEventArgs<RplMyInfoMessage> e)
        {
            RplMyInfo?.Invoke(client, e);
        }

        public event IRCMessageEventHandler<RplISupportMessage> RplISupport;
        internal void OnRplISupport(IRCMessageEventArgs<RplISupportMessage> e)
        {
            RplISupport?.Invoke(client, e);
        }
    }
}
