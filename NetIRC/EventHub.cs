using NetIRC.Messages;

namespace NetIRC
{
    public class EventHub
    {
        public event IRCMessageEventHandler<DefaultIRCMessage> Default;
        internal void OnDefault(IRCMessageEventArgs<DefaultIRCMessage> e)
        {
            Default?.Invoke(this, e);
        }

        public event IRCMessageEventHandler<PingCommand> Ping;
        internal void OnPing(IRCMessageEventArgs<PingCommand> e)
        {
            Ping?.Invoke(this, e);
        }

        public event IRCMessageEventHandler<PrivMsgMessage> PrivMsg;
        internal void OnPrivMsg(IRCMessageEventArgs<PrivMsgMessage> e)
        {
            PrivMsg?.Invoke(this, e);
        }
    }
}