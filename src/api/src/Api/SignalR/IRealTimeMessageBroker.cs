namespace Api.Messaging
{
    public interface IRealTimeMessageBroker
    {
        Task SendMessage<T>(string connection, T message);
    }
}
