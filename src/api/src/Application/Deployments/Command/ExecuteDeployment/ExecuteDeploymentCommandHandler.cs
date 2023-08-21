using Domain.Deployments;
using Domain.Events;
using Domain.Interfaces;
using Domain.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Deployments.Command.ExecuteDeployment
{
    public class ExecuteDeploymentCommandHandler : IRequestHandler<ExecuteDeploymentCommand, Unit>
    {
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly ICodeDeploymentService _codeDeploymentService;
        private readonly IEnvironmentDeploymentService _environmentDeploymentService;
        private readonly IDeploymentEventService _deploymentEventService;
        private readonly ILogger<ExecuteDeploymentCommandHandler> _logger;

        public ExecuteDeploymentCommandHandler(
            IDeploymentRepository deploymentRepository,
            ICodeDeploymentService codeDeploymentService,
            IEnvironmentDeploymentService environmentDeploymentService,
            ILogger<ExecuteDeploymentCommandHandler> logger, IDeploymentEventService deploymentEventService)
        {
            _deploymentRepository = deploymentRepository;
            _codeDeploymentService = codeDeploymentService;
            _environmentDeploymentService = environmentDeploymentService;
            _logger = logger;
            _deploymentEventService = deploymentEventService;
        }

        public async Task<Unit> Handle(ExecuteDeploymentCommand request, CancellationToken cancellationToken)
        {
            var deployment = await _deploymentRepository.GetAsync(request.Id, cancellationToken);
            try
            {
                await _deploymentEventService.SaveEvent(new DeploymentStarted(request.Id), cancellationToken);

                if (deployment == null)
                {
                    await _deploymentEventService.SaveEvent(new DeploymentNotFound(request.Id), cancellationToken);

                    return Unit.Value;
                }

                var jobs = new List<Task>();
                if (deployment.CodeDeployments != null)
                {
                    jobs.Add(DeployCodeAsync(deployment.CodeDeployments, deployment.Id, cancellationToken));
                };

                foreach (var env in deployment.EnvironmentDeployments)
                {
                    jobs.Add(_environmentDeploymentService.DeployEnvironment(env, deployment.Id, cancellationToken));
                }

                await Task.WhenAll(jobs);

                deployment.FinishDeployment();

                await _deploymentRepository.UpdateAsync(deployment, cancellationToken);
                await _deploymentEventService.SaveEvent(new DeploymentFinished(request.Id), cancellationToken);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                deployment.FailureDeployment();
                await _deploymentRepository.UpdateAsync(deployment, cancellationToken);
                await _deploymentEventService.SaveEvent(new DeploymentFailed(request.Id), cancellationToken);

                return Unit.Value;
            }

        }

        private async Task DeployCodeAsync(IEnumerable<CodeDeployment> codeDeployments, Guid deploymentId, CancellationToken ct)
        {
            foreach (var codeDeployment in codeDeployments)
            {
                await _codeDeploymentService.DeployCode(codeDeployment, deploymentId, ct);
            }
        }
    }
}
