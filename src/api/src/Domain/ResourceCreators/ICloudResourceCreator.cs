using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface ICloudResourceCreator
    {
        Task<CloudCreatorResponse<string>> CreateResourceGroupAsync(ResourceGroupDeployment resourceGroup, CancellationToken ct);
        Task<CloudCreatorResponse<string>> DeployResourceInResourceGroupAsync(ResourceDeployment resourceDeployment, ResourceGroupDeployment resourceGroup, CancellationToken ct);
    }
}
