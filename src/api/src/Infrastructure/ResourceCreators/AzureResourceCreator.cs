using Azure.ResourceManager.Resources.Models;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.Core;
using Azure;
using Domain.Interfaces;
using Domain.Deployments;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ResourceCreators
{
    public class AzureResourceCreator : ICloudResourceCreator
    {
        private readonly ArmClient _armClient;
        private readonly ILogger<AzureResourceCreator> _logger;

        public AzureResourceCreator(ArmClient armClient, ILogger<AzureResourceCreator> logger)
        {
            _armClient = armClient;
            _logger = logger;
        }

        public async Task<CloudCreatorResponse<string>> CreateResourceGroupAsync(ResourceGroupDeployment resourceGroup, CancellationToken ct)
        {
            try
            {
                SubscriptionResource subscription = await _armClient.GetDefaultSubscriptionAsync(ct);

                ResourceGroupCollection rgCollection = subscription.GetResourceGroups();

                var location = new AzureLocation(resourceGroup.Location);
                var response = await rgCollection.CreateOrUpdateAsync(WaitUntil.Completed, resourceGroup.Name, new ResourceGroupData(location), ct);
                if (resourceGroup.Tags != null)
                {
                    await response.Value.SetTagsAsync(resourceGroup.Tags);
                }

                return new CloudCreatorResponse<string>(response.Value.Id, BuildUrlToResource(response.Value.Id, subscription.Data.TenantId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return new CloudCreatorResponse<string>(ex.Message);
            }
        }

        public async Task<CloudCreatorResponse<string>> DeployResourceInResourceGroupAsync(ResourceDeployment resourceDeployment, ResourceGroupDeployment resourceGroup, CancellationToken ct)
        {
            try
            {
                var subscription = await _armClient.GetDefaultSubscriptionAsync(ct);
                var resourceGroupResponse = subscription.GetResourceGroup(resourceGroup.Name, ct);
                var existingResourceGroup = resourceGroupResponse.Value;

                var armDeploymentCollection = existingResourceGroup.GetArmDeployments();

                var input = new ArmDeploymentContent(new ArmDeploymentProperties(ArmDeploymentMode.Incremental)
                {
                    Template = BinaryData.FromString(resourceDeployment.Template),
                    Parameters = BinaryData.FromString(JObject.FromObject(resourceDeployment.Parameters).ToString())
                });

                var result = await armDeploymentCollection.CreateOrUpdateAsync(WaitUntil.Completed, resourceDeployment.Name, input, ct);

                GenericResource createdResource = null;
                await foreach (var resource in existingResourceGroup.GetGenericResourcesAsync(cancellationToken: ct))
                {
                    if (resource.Data.Name == resourceDeployment.Name)
                    {
                        createdResource = resource;
                        if (resourceGroup.Tags != null)
                        {
                            try
                            {
                                await createdResource.SetTagsAsync(resourceGroup.Tags, ct);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                            }
                        }
                    }
                }

                return new CloudCreatorResponse<string>(createdResource.Data.Id, BuildUrlToResource(createdResource.Data.Id, subscription.Data.TenantId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExceptionMessage: {exceptionMessage}", ex.Message);
                return new CloudCreatorResponse<string>(ex.Message);
            }
        }

        private static string BuildUrlToResource(string resourceId, Guid tenantId)
        {
            return $"https://portal.azure.com/#@{tenantId}/resource{resourceId}";
        }
    }
}
