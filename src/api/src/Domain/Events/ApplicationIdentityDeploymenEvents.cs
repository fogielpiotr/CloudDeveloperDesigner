using Domain.Deployments;

namespace Domain.Events
{
    public class ApplicationIdentityDeploymentStarted : DeploymentEvent
    {
        public ApplicationIdentityDeploymentStarted(ApplicationIdentityDeployment applicationIdentityDeployment, string env, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = applicationIdentityDeployment.Name;
            Message = $"Application Identity: '{Name}' deployment started.";
            Environment = env;
            Status = DeploymentStatus.OperationStarted;
        }
    }

    public class ApplicationIdentityDeploymentSucceeded : DeploymentEvent
    {
        public ApplicationIdentityDeploymentSucceeded(ApplicationIdentityDeployment applicationIdentityDeployment, string env, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = applicationIdentityDeployment.Name;
            Id = applicationIdentityDeployment.CloudIdentifier;
            Environment = env;
            Message = $"Application Identity: '{Name}' deployment succeeded.";
            Url = applicationIdentityDeployment.Url;
            Status = DeploymentStatus.OperationSucceeded;
        }
    }

    public class ApplicationIdentityDeploymentFailed : DeploymentEvent
    {
        public ApplicationIdentityDeploymentFailed(ApplicationIdentityDeployment applicationIdentityDeployment, string env, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = applicationIdentityDeployment.Name;
            Environment = env;
            Message = $"Application Identity: '{Name}' deployment failed.";
            Error = applicationIdentityDeployment.ErrorMessage;
            Status = DeploymentStatus.OperationFailed;
        }
    }
}
