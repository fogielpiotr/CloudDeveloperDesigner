using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IApplicationIdentityService
    {
        Task DeployApplicationIdentity(ApplicationIdentityDeployment appIdentity, CancellationToken ct);
        Task<bool> AddAuthorizedApp(ApplicationIdentityDeployment appIdentity, IEnumerable<ApplicationIdentityDeployment> authorizedApps, CancellationToken ct);
        Task<string> GetApplicationIndetitySecrets(ApplicationIdentityDeployment appIdentity, CancellationToken ct);
    }
}
