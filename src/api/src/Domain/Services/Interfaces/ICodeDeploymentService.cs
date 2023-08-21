using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface ICodeDeploymentService
    {
        Task DeployCode(CodeDeployment codeDeployment, Guid deploymentId, CancellationToken ct);
    }
}
