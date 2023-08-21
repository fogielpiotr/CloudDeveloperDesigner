using Domain.Deployments;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace Infrastructure.ResourceCreators
{
    public class AzureAdApplicationCreator : IApplicationIdentityCreator
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly ILogger<AzureAdApplicationCreator> _logger;

        public AzureAdApplicationCreator(GraphServiceClient graphServiceClient, ILogger<AzureAdApplicationCreator> logger)
        {
            _graphServiceClient = graphServiceClient;
            _logger = logger;
        }

        public async Task<CloudCreatorResponse<ApplicationIdentityIdentifier>> DeployServerApplication(ApplicationIdentityDeployment appDeployment, CancellationToken ct)
        {
            try
            {
                var app = new Microsoft.Graph.Application
                {
                    DisplayName = appDeployment.Name,
                    IdentifierUris = new[] { $"api://{appDeployment.Name}" },
                    RequiredResourceAccess = new List<RequiredResourceAccess>
                    {
                        new RequiredResourceAccess
                        {
                           ResourceAppId = "00000003-0000-0000-c000-000000000000",
                           ResourceAccess = new List<ResourceAccess>
                           {
                               new ResourceAccess
                               {
                                   Id = Guid.Parse("e1fe6dd8-ba31-4d61-89e7-88639da4683d"),
                                   Type = "Scope"
                               }
                           }
                        }
                    },
                    Api = new ApiApplication()
                    {
                        Oauth2PermissionScopes = new[] { new PermissionScope
                        {
                                    Id = Guid.NewGuid(),
                                    AdminConsentDescription = "Allow access to application",
                                    AdminConsentDisplayName = "Allow access to application",
                                    IsEnabled = true,
                                    Type = "User",
                                    Value = "Access"
                        }}
                    }

                };

                var createdApp = await _graphServiceClient.Applications
                    .Request()
                    .AddAsync(app, ct);

                var webUrl = $"https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/{createdApp.AppId}";

                return new CloudCreatorResponse<ApplicationIdentityIdentifier>(
                    new ApplicationIdentityIdentifier(createdApp.Id, createdApp.AppId),
                    webUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return new CloudCreatorResponse<ApplicationIdentityIdentifier>(ex.Message);
            }
        }

        public async Task<CloudCreatorResponse<ApplicationIdentityIdentifier>> DeployWebApplication(ApplicationIdentityDeployment appDeployment, CancellationToken ct)
        {
            try
            {
                var app = new Microsoft.Graph.Application
                {
                    DisplayName = appDeployment.Name,
                    RequiredResourceAccess = new List<RequiredResourceAccess>
                    {
                        new RequiredResourceAccess
                        {
                           ResourceAppId = "00000003-0000-0000-c000-000000000000",
                           ResourceAccess = new List<ResourceAccess>
                           {
                               new ResourceAccess
                               {
                                   Id = Guid.Parse("e1fe6dd8-ba31-4d61-89e7-88639da4683d"),
                                   Type = "Scope"
                               }
                           }
                        }
                    },
                    Spa = new SpaApplication
                    {
                        RedirectUris = appDeployment.RedirectUris
                    }
                };

                var createdApp = await _graphServiceClient.Applications
                    .Request()
                    .AddAsync(app, ct);

                var webUrl = $"https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/Overview/appId/{createdApp.AppId}";

                return new CloudCreatorResponse<ApplicationIdentityIdentifier>(
                  new ApplicationIdentityIdentifier(createdApp.Id, createdApp.AppId),
                  webUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return new CloudCreatorResponse<ApplicationIdentityIdentifier>(ex.Message);
            }

        }

        public async Task<string> CreateSecretsAsync(ApplicationIdentityDeployment appDeployment, CancellationToken ct)
        {
            try
            {
                var passwordCredential = new PasswordCredential
                {
                    DisplayName = $"{appDeployment.Name}Secret"
                };
                var passwrodResponse = await _graphServiceClient.Applications[appDeployment.CloudIdentifier.ObjectId]
                .AddPassword(passwordCredential)
                .Request()
                .PostAsync(ct);

                return passwrodResponse.SecretText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return string.Empty;
            }
        }

        public async Task<bool> AddAuthorizedApplications(ApplicationIdentityDeployment appDeployment, IEnumerable<ApplicationIdentityDeployment> authorizedApps, CancellationToken ct)
        {
            try
            {
                var app = await _graphServiceClient.Applications[appDeployment.CloudIdentifier.ObjectId].Request().GetAsync(ct);

                var preAuthorizedApplications = new List<PreAuthorizedApplication>();
                foreach (var connectedApp in authorizedApps.Select(x => x.CloudIdentifier.ApplicationId))
                {
                    preAuthorizedApplications.Add(new PreAuthorizedApplication
                    {
                        AppId = connectedApp,
                        DelegatedPermissionIds = new[] { app.Api.Oauth2PermissionScopes.First().Id.Value.ToString() }
                    });
                }
                app.Api.PreAuthorizedApplications = preAuthorizedApplications;

                await _graphServiceClient.Applications[appDeployment.CloudIdentifier.ObjectId].Request().UpdateAsync(app, ct);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return false;
            }
        }
    }
}
