using Domain.Deployments;

namespace Domain.Events
{
    public class ResourceGroupDeploymentStarted : DeploymentEvent
    {
        public ResourceGroupDeploymentStarted(ResourceGroupDeployment resourceGroup, string environment, Guid deploymentId)
        {
            Environment = environment;
            DeploymentId = deploymentId;
            Name = resourceGroup.Name;
            Message = $"Resource Group: '{Name}' deployment started.";
            Status = DeploymentStatus.OperationStarted;
        }
    }

    public class ResourceGroupDeploymentSucceeded : DeploymentEvent
    {
        public ResourceGroupDeploymentSucceeded(ResourceGroupDeployment resourceGroup, string environment, Guid deploymentId)
        {
            Id = resourceGroup.CloudIdentifier;
            Url = resourceGroup.Url;
            Environment = environment;
            DeploymentId = deploymentId;
            Name = resourceGroup.Name;
            Message = $"Resource Group: '{Name}' deployment succeeded.";
            Status = DeploymentStatus.OperationSucceeded;
        }
    }

    public class ResourceGroupDeploymentFailed : DeploymentEvent
    {
        public ResourceGroupDeploymentFailed(ResourceGroupDeployment resourceGroup, string environment, Guid deploymentId)
        {
            Environment = environment;
            DeploymentId = deploymentId;
            Name = resourceGroup.Name;
            Message = $"Resource Group: '{Name}' deployment failed.";
            Error = resourceGroup.ErrorMessage;
            Status = DeploymentStatus.OperationFailed;
        }
    }
}
