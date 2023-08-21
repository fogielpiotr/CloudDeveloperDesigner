using Domain.Deployments;

namespace Domain.Events
{
    public class ResourceDeploymentStarted : DeploymentEvent
    {
        public ResourceDeploymentStarted(ResourceDeployment resource, string env, Guid deploymentId)
        {
            Environment = env;
            DeploymentId = deploymentId;
            Name = resource.Name;
            Message = $"Resource: '{Name}' deployment started.";
            Status = DeploymentStatus.OperationStarted;
        }
    }

    public class ResourceDeploymentSucceeded : DeploymentEvent
    {
        public ResourceDeploymentSucceeded(ResourceDeployment resource, string env, Guid deploymentId)
        {
            Id = resource.CloudIdentifier;
            Url = resource.Url;
            Environment = env;
            DeploymentId = deploymentId;
            Name = resource.Name;
            Message = $"Resource: '{Name}' deployment succeeded.";
            Status = DeploymentStatus.OperationSucceeded;
        }
    }

    public class ResourceDeploymentFailed : DeploymentEvent
    {
        public ResourceDeploymentFailed(ResourceDeployment resource, string env, Guid deploymentId)
        {
            Environment = env;
            DeploymentId = deploymentId;
            Name = resource.Name;
            Message = $"Resource: '{Name}' deployment failed.";
            Error = resource.ErrorMessage;
            Status = DeploymentStatus.OperationFailed;
        }
    }
}
