using Azure.ResourceManager;
using Azure.ResourceManager.CosmosDB;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ResourceManagement
{
    public class CosmosDbManager : IResourceManager
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<CosmosDbManager> _logger;

        public CosmosDbManager(ArmClient armClient, ILogger<CosmosDbManager> logger)
        {
            _armClient = armClient;
            _logger = logger;
        }

        public string Name => "Microsoft.DocumentDB/databaseAccounts";

        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var subscription = await _armClient.GetDefaultSubscriptionAsync(ct);
                var rgResponse = await subscription.GetResourceGroupAsync(resourceGroupName);
                var resourceGroup = rgResponse.Value;

                var cosmosResponse = await resourceGroup.GetDatabaseAccountAsync(resourceName);
                var cosmosResource = cosmosResponse.Value;
                var cosmosKeysResponse = await cosmosResource.GetKeysAsync();
                var cosmosKeys = cosmosKeysResponse.Value;

                return $"AccountEndpoint={cosmosResource.Data.DocumentEndpoint};AccountKey={cosmosKeys.PrimaryMasterKey}";

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);

                return string.Empty;
            }
        }
    }
}
