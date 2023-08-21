using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IEnvironmentDeploymentService
    {
        Task DeployEnvironment(EnvironmentDeployment environmentDeployment, Guid deploymentId, CancellationToken ct);
    }
}
