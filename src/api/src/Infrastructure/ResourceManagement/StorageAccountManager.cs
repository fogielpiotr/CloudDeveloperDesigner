using Azure.ResourceManager;
using Azure.ResourceManager.Storage;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ResourceManagement
{
    public class StorageAccountManager : IResourceManager
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<StorageAccountManager> _logger;

        public StorageAccountManager(ArmClient armClient, ILogger<StorageAccountManager> logger)
        {
            _armClient = armClient;
            _logger = logger;
        }

        public string Name => "microsoft.storage/storageaccounts";

        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var subscription = await _armClient.GetDefaultSubscriptionAsync(ct);
                var rgResponse = await subscription.GetResourceGroupAsync(resourceGroupName, ct);
                var resourceGroup = rgResponse.Value;

                var storageResponse = await resourceGroup.GetStorageAccountAsync(resourceName, cancellationToken: ct);
                StorageAccountResource storageAccount = storageResponse.Value;
                var storageAccountKeysResponse = await storageAccount.GetKeysAsync(ct);
                var storagaAccountKeys = storageAccountKeysResponse.Value.Keys;

                return $"DefaultEndpointsProtocol=https;AccountName={resourceName};AccountKey={storagaAccountKeys[0].Value};EndpointSuffix=core.windows.net";
   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return string.Empty;
            }
        }
    }
}
