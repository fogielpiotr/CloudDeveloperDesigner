using Domain.Deployments;

namespace Domain.Events
{
    public class CodeBuildDeploymentStarted : DeploymentEvent
    {
        public CodeBuildDeploymentStarted(BuildDeployment buildDeployment, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = buildDeployment.Name;
            CodeDeployment = true;
            Message = $"Code Build: '{Name}' deployment started.";
            Status = DeploymentStatus.OperationStarted;
        }
    }

    public class CodeBuildDeploymentSucceeded : DeploymentEvent
    {
        public CodeBuildDeploymentSucceeded(BuildDeployment buildDeployment, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = buildDeployment.Name;
            Id = buildDeployment.CloudIdentifier;
            Url = buildDeployment.Url;
            CodeDeployment = true;
            Message = $"Code Build: '{Name}' deployment succeeded.";
            Status = DeploymentStatus.OperationSucceeded;
        }
    }

    public class CodeBuildDeploymentFailed : DeploymentEvent
    {
        public CodeBuildDeploymentFailed(BuildDeployment buildDeployment, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = buildDeployment.Name;
            CodeDeployment = true;
            Message = $"Code Build: '{Name}' deployment failed.";
            Error = buildDeployment.ErrorMessage;
            Status = DeploymentStatus.OperationFailed;
        }
    }
}
