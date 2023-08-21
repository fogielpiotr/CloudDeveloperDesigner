using Azure.ResourceManager;
using Azure.ResourceManager.ServiceBus;
using Domain;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ResourceManagement
{
    public class ServiceBusManager : IResourceManager
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<ServiceBusManager> _logger;

        public ServiceBusManager(ArmClient armClient, ILogger<ServiceBusManager> logger)
        {
            _armClient = armClient;
            _logger = logger;
        }

        public string Name => "Microsoft.ServiceBus/namespaces";

        public async Task<string> GetResourceSecret(string resourceGroupName, string resourceName, CancellationToken ct)
        {
            try
            {
                var subscription = await _armClient.GetDefaultSubscriptionAsync(ct);
                var rgResponse = await subscription.GetResourceGroupAsync(resourceGroupName, ct);
                var resourceGroup = rgResponse.Value;

                var busResponse = await resourceGroup.GetServiceBusNamespaceAsync(resourceName, ct);
                var busNampesace = busResponse.Value;

                var authRules = busNampesace.GetNamespaceAuthorizationRules();
                var authRule = authRules.First();
                var authRuleKeysResponse = await authRule.GetKeysAsync(ct);
                var busNamespaceKeys = authRuleKeysResponse.Value;

                return busNamespaceKeys.PrimaryConnectionString;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                
                return string.Empty;
            }
        }
    }
}
