using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IApplicationIdentityCreator
    {
        public Task<CloudCreatorResponse<ApplicationIdentityIdentifier>> DeployServerApplication(ApplicationIdentityDeployment appDeployment, CancellationToken ct);
        public Task<CloudCreatorResponse<ApplicationIdentityIdentifier>> DeployWebApplication(ApplicationIdentityDeployment appDeployment, CancellationToken ct);
        public Task<string> CreateSecretsAsync(ApplicationIdentityDeployment appDeployment, CancellationToken ct);
        public Task<bool> AddAuthorizedApplications(ApplicationIdentityDeployment appDeployment, IEnumerable<ApplicationIdentityDeployment> authorizedApps, CancellationToken ct);
    }
}
