using Domain.Deployments;
using Domain.Events;
using Domain.Services.Interfaces;

namespace Domain.Services
{
    public class DeploymentEventService : IDeploymentEventService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IDeploymentLogsRepository _deploymentLogsRepository;

        public DeploymentEventService(IMessageBroker messageBroker, IDeploymentLogsRepository deploymentLogsRepository)
        {
            _messageBroker = messageBroker;
            _deploymentLogsRepository = deploymentLogsRepository;
        }

        public async Task SaveEvent(DeploymentEvent @event, CancellationToken ct)
        {
            var deploymentLog = new DeploymentLog(
                    Guid.NewGuid(),
                    @event.Date,
                    @event.DeploymentId,
                    @event.Message,
                    @event.Url,
                    @event.Environment,
                    @event.CodeDeployment,
                    @event.Name,
                    @event.Error,
                    @event.Status
            );

            await _deploymentLogsRepository.AddAsync(deploymentLog, ct);
            await _messageBroker.SendMessage(@event, ct);
        }
    }
}
