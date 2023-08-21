using Domain.Applications;
using Domain.Deployments;
using Domain.Events;
using Domain.Interfaces;
using Domain.Services.Interfaces;

namespace Domain.Services
{
    public class EnvironmentDeploymentService : IEnvironmentDeploymentService
    {
        private readonly IDeploymentEventService _eventService;
        private readonly IResourceDeploymentService _resourceDeploymentService;
        private readonly IApplicationIdentityService _applicationIdentityService;
        private readonly ISecretsCreator _secretsCreator;

        public EnvironmentDeploymentService(
            IDeploymentEventService eventService,
            IResourceDeploymentService resourceDeploymentService,
            IApplicationIdentityService applicationIdentityService,
            ISecretsCreator secretsCreator)
        {
            _resourceDeploymentService = resourceDeploymentService;
            _applicationIdentityService = applicationIdentityService;
            _secretsCreator = secretsCreator;
            _eventService = eventService;
        }

        public async Task DeployEnvironment(EnvironmentDeployment deployment, Guid deploymentId, CancellationToken ct)
        {
            ResourceDeployment secretProvider = null;
            var secretsDictionary = new Dictionary<string, string>();
            if (deployment.ResourceDeployments != null && deployment.ResourceDeployments.Any())
            {
                await _eventService.SaveEvent(new ResourceGroupDeploymentStarted(deployment.ResourceGroup, deployment.Environment, deploymentId), ct);
                await _resourceDeploymentService.DeployResourceGroup(deployment.ResourceGroup, ct);
                if (!deployment.ResourceGroup.Success)
                {
                    await _eventService.SaveEvent(new ResourceGroupDeploymentFailed(deployment.ResourceGroup, deployment.Environment, deploymentId), ct);

                    return;
                }
                await _eventService.SaveEvent(new ResourceGroupDeploymentSucceeded(deployment.ResourceGroup, deployment.Environment, deploymentId), ct);


                secretProvider = deployment.ResourceDeployments.FirstOrDefault(x => x.IsSercretProvider);
                if (secretProvider != null)
                {
                    await _eventService.SaveEvent(new ResourceDeploymentStarted(secretProvider, deployment.Environment, deploymentId), ct);
                    await _resourceDeploymentService.DeployResourceInResourceGroup(secretProvider, deployment.ResourceGroup, ct);
                    if (!secretProvider.Success)
                    {
                        await _eventService.SaveEvent(new ResourceDeploymentFailed(secretProvider, deployment.Environment, deploymentId), ct);

                        return;
                    }
                    await _eventService.SaveEvent(new ResourceDeploymentSucceeded(secretProvider, deployment.Environment, deploymentId), ct);
                }

                foreach (var deploymentResource in deployment.ResourceDeployments.Where(x => !x.IsSercretProvider))
                {
                    await _eventService.SaveEvent(new ResourceDeploymentStarted(deploymentResource, deployment.Environment, deploymentId), ct);
                    await _resourceDeploymentService.DeployResourceInResourceGroup(deploymentResource, deployment.ResourceGroup, ct);
                    if (!deploymentResource.Success)
                    {
                        await _eventService.SaveEvent(new ResourceDeploymentFailed(deploymentResource, deployment.Environment, deploymentId), ct);

                        continue;
                    }
                    await _eventService.SaveEvent(new ResourceDeploymentSucceeded(deploymentResource, deployment.Environment, deploymentId), ct);
                    if (secretProvider != null)
                    {
                        var secretValue = await _resourceDeploymentService.GetResourceSecret(deploymentResource, deployment.ResourceGroup, ct);
                        if (!string.IsNullOrWhiteSpace(secretValue))
                        {
                            secretsDictionary.Add(deploymentResource.SecretName, secretValue);
                        }
                    }
                }
            }

            if (deployment.ApplicationIdentityDeployments != null)
            {
                foreach (var appIdentity in deployment.ApplicationIdentityDeployments)
                {
                    await _eventService.SaveEvent(new ApplicationIdentityDeploymentStarted(appIdentity, deployment.Environment, deploymentId), ct);
                    await _applicationIdentityService.DeployApplicationIdentity(appIdentity, ct);
                    if (!appIdentity.Success)
                    {
                        await _eventService.SaveEvent(new ApplicationIdentityDeploymentFailed(appIdentity, deployment.Environment, deploymentId), ct);
                        continue;
                    }
                    await _eventService.SaveEvent(new ApplicationIdentityDeploymentSucceeded(appIdentity, deployment.Environment, deploymentId), ct);
                    if (secretProvider != null)
                    {
                        var secretValue = await _applicationIdentityService.GetApplicationIndetitySecrets(appIdentity, ct);
                        if (!string.IsNullOrWhiteSpace(secretValue))
                        {
                            secretsDictionary.Add(appIdentity.ClientSecretName, secretValue);
                        }
                        secretsDictionary.Add(appIdentity.ClientIdSecretName, appIdentity.CloudIdentifier.ApplicationId);
                    }
                }

                foreach (var appIdentity in deployment.ApplicationIdentityDeployments.Where(x => x.Type == ApplicationType.Server))
                {
                    var connectedApps = deployment.ApplicationIdentityDeployments.Where(x => x.Success && appIdentity.AuthorizedApps.Contains(x.Name));
                    await _applicationIdentityService.AddAuthorizedApp(appIdentity, connectedApps, ct);
                }
            }

            if (secretProvider != null)
            {
                await _secretsCreator.AddSecrets(secretProvider.Name, secretsDictionary);
            }
        }
    }
}
