using Domain.Deployments;
using Domain.Interfaces;

namespace Domain.Services
{
    public class ApplicationIdentityService : IApplicationIdentityService
    {
        private readonly IApplicationIdentityCreator _applicationIdentityCreator;

        public ApplicationIdentityService(IApplicationIdentityCreator applicationIdentityCreator)
        {
            _applicationIdentityCreator = applicationIdentityCreator;
        }

        public async Task<bool> AddAuthorizedApp(ApplicationIdentityDeployment appIdentity, IEnumerable<ApplicationIdentityDeployment> authorizedApps, CancellationToken ct)
        {
            if (appIdentity.AuthorizedApps.Any())
            {
                return await _applicationIdentityCreator.AddAuthorizedApplications(appIdentity, authorizedApps, ct);
            }
            return true;

        }

        public async Task DeployApplicationIdentity(ApplicationIdentityDeployment appIdentity, CancellationToken ct)
        {
            CloudCreatorResponse<ApplicationIdentityIdentifier> response = null;
            switch (appIdentity.Type)
            {
                case Applications.ApplicationType.Web:
                    response = await _applicationIdentityCreator.DeployWebApplication(appIdentity, ct);
                    break;
                case Applications.ApplicationType.Server:
                    response = await _applicationIdentityCreator.DeployServerApplication(appIdentity, ct);
                    break;
            }
            if (!response.Success)
            {
                appIdentity.SetFailure(response.ErrorMessage);
            }
            else
            {
                appIdentity.SetSuccess(response);
            }
        }

        public async Task<string> GetApplicationIndetitySecrets(ApplicationIdentityDeployment appIdentity, CancellationToken ct)
        {
            if (appIdentity.Type == Applications.ApplicationType.Server)
            {
                return await _applicationIdentityCreator.CreateSecretsAsync(appIdentity, ct);
            }

            return string.Empty;
        }
    }
}
