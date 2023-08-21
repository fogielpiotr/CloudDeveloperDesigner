using Domain.Deployments;
using Domain.Events;
using Domain.Interfaces;
using Domain.Services.Interfaces;

namespace Domain.Services
{
    public class CodeDeploymentService : ICodeDeploymentService
    {
        private readonly IRemoteCodeRepositoryCreator _remoteCodeCreator;
        private readonly IBuildCreator _buildCreator;
        private readonly IDeploymentEventService _deploymentEventService;

        public CodeDeploymentService(
            IRemoteCodeRepositoryCreator codeRepository,
            IBuildCreator buildRepository,
            IDeploymentEventService deploymentEventService)
        {
            _remoteCodeCreator = codeRepository;
            _buildCreator = buildRepository;
            _deploymentEventService = deploymentEventService;
        }

        public async Task DeployCode(CodeDeployment codeDeployment, Guid deploymentId, CancellationToken ct)
        {
            await _deploymentEventService.SaveEvent(new CodeRepositoryDeploymentStarted(codeDeployment.RepositoryDeployment, deploymentId), ct);
            await DeployRemoteRepositoryAsync(codeDeployment.RepositoryDeployment, ct);
            if (!codeDeployment.RepositoryDeployment.Success)
            {
                await _deploymentEventService.SaveEvent(new CodeRepositoryDeploymentFailed(codeDeployment.RepositoryDeployment, deploymentId), ct);

                return;
            }
            await _deploymentEventService.SaveEvent(new CodeRepositoryDeploymentSucceeded(codeDeployment.RepositoryDeployment, deploymentId), ct);

            await _deploymentEventService.SaveEvent(new CodeBuildDeploymentStarted(codeDeployment.BuildDeployment, deploymentId), ct);
            await DeployBuildForRepository(codeDeployment.BuildDeployment, codeDeployment.RepositoryDeployment, ct);
            if (!codeDeployment.BuildDeployment.Success)
            {
                await _deploymentEventService.SaveEvent(new CodeBuildDeploymentFailed(codeDeployment.BuildDeployment, deploymentId), ct);
                return;
            }
            await _deploymentEventService.SaveEvent(new CodeBuildDeploymentSucceeded(codeDeployment.BuildDeployment, deploymentId), ct);
        }

        private async Task DeployRemoteRepositoryAsync(RepositoryDeployment repositoryDeployment, CancellationToken ct)
        {
            var response = await _remoteCodeCreator.CreateRemoteCodeRepositoryAsync(repositoryDeployment, ct);
            if (!response.Success)
            {
                repositoryDeployment.SetFailure(response.ErrorMessage);
            }
            else
            {
                repositoryDeployment.SetSuccess(response);
            }
        }

        private async Task DeployBuildForRepository(BuildDeployment buildDeployment, RepositoryDeployment repositoryDeployment, CancellationToken ct)
        {
            var buildResponse = await _buildCreator.CreateAndQueueBuildForRepository(buildDeployment, repositoryDeployment, ct);
            if (!buildResponse.Success)
            {
                buildDeployment.SetFailure(buildResponse.ErrorMessage);
            }
            else
            {
                buildDeployment.SetSuccess(buildResponse);
            }
        }
    }
}
