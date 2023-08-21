using Azure.ResourceManager;
using Azure.ResourceManager.AppConfiguration;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ResourceManagement
{
    public class AppConfigurationManager : IResourceManager
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<AppConfigurationManager> _logger;

        public AppConfigurationManager(ArmClient armClient, ILogger<AppConfigurationManager> logger)
        {
            _armClient = armClient;
            _logger = logger;
        }

        public string Name => "Microsoft.AppConfiguration/configurationStores";

        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var subscription = await _armClient.GetDefaultSubscriptionAsync(ct);
                var rgResponse = await subscription.GetResourceGroupAsync(resourceGroupName);
                var resourceGroup = rgResponse.Value;

                var appConfigurationResponse = await resourceGroup.GetConfigurationStoreAsync(resourceName, ct);
                var appConfiguration = appConfigurationResponse.Value;

                var connectionStrings = new List<string>();
                await foreach (var key in appConfiguration.GetKeysAsync())
                {
                    connectionStrings.Add(key.ConnectionString);
                }

                return connectionStrings.First();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                
                return string.Empty;
            }
        }
    }
}
