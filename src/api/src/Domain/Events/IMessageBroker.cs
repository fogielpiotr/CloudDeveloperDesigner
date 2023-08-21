namespace Domain.Events
{
    public interface IMessageBroker
    {
        Task SendMessage<T>(T message, CancellationToken ct);
    }
}
