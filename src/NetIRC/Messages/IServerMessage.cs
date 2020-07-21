namespace GravyIrc.Messages
{
    public interface IServerMessage
    {
        void TriggerEvent(EventHub eventHub);
    }
}
