using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IResourceDeploymentService
    {
        Task DeployResourceInResourceGroup(ResourceDeployment resource, ResourceGroupDeployment resourceGroup, CancellationToken ct);
        Task DeployResourceGroup(ResourceGroupDeployment resourceGroup, CancellationToken ct);
        Task<string> GetResourceSecret(ResourceDeployment resource, ResourceGroupDeployment resourceGroup, CancellationToken ct);
    }
}
