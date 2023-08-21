using Domain.Deployments;
using Domain.Interfaces;

namespace Domain.Services
{
    public class ResourceDeploymentService : IResourceDeploymentService
    {
        private readonly ICloudResourceCreator _cloudResourceCreator;
        private readonly IResourceManagerFactory _resourceManagerFactory;

        public ResourceDeploymentService(
            ICloudResourceCreator cloudResourceRepository,
            IResourceManagerFactory resourceManagerFactory)
        {
            _cloudResourceCreator = cloudResourceRepository;
            _resourceManagerFactory = resourceManagerFactory;
        }

        public async Task DeployResourceGroup(ResourceGroupDeployment resourceGroup, CancellationToken ct)
        {
            var response = await _cloudResourceCreator.CreateResourceGroupAsync(resourceGroup, ct);
            if (!response.Success)
            {
                resourceGroup.SetFailure(response.ErrorMessage);
            }
            else
            {
                resourceGroup.SetSuccess(response);
            }
        }

        public async Task DeployResourceInResourceGroup(ResourceDeployment resource, ResourceGroupDeployment resourceGroup, CancellationToken ct)
        {
            var response = await _cloudResourceCreator.DeployResourceInResourceGroupAsync(resource, resourceGroup, ct);
            if (!response.Success)
            {
                resource.SetFailure(response.ErrorMessage);
            }
            else
            {
                resource.SetSuccess(response);
            }
        }

        public async Task<string> GetResourceSecret(ResourceDeployment resource, ResourceGroupDeployment resourceGroup, CancellationToken ct)
        {
            var resourceManager = _resourceManagerFactory.GetManager(resource.ResourceName);
            if (resourceManager != null)
            {
                return await resourceManager.GetResourceSecret(resourceGroup.Name, resource.Name, ct);
            }

            return string.Empty;
        }
    }
}
