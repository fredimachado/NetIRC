using NetIRC.Messages;

namespace NetIRC
{
    public class EventHub
    {
        private readonly Client client;

        public EventHub(Client client)
        {
            this.client = client;
        }

        public event IRCMessageEventHandler<DefaultIRCMessage> Default;
        internal void OnDefault(IRCMessageEventArgs<DefaultIRCMessage> e)
        {
            Default?.Invoke(client, e);
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
    }
}
