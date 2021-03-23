using System.Threading.Tasks;

namespace NetIRC.Messages
{
    public abstract class MessageHandler<TServerMessage> : IMessageHandler<TServerMessage>
        where TServerMessage : IServerMessage
    {
        public TServerMessage Message { get; internal set; }

        public abstract Task HandleAsync(TServerMessage serverMessage, Client client);
    }
}
