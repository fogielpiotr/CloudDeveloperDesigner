using Domain.Events;

namespace Domain.Services.Interfaces
{
    public interface IDeploymentEventService
    {
        Task SaveEvent(DeploymentEvent @event, CancellationToken ct);
    }
}
