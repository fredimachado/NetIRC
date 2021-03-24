using System.Threading.Tasks;

namespace NetIRC.Messages
{
    internal interface IMessageHandler<TServerMessage>
        where TServerMessage : IServerMessage
    {
        TServerMessage Message { get; }
        Task HandleAsync(TServerMessage serverMessage, Client client);
    }
}
