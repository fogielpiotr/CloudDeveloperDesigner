using Domain.Deployments;

namespace Domain.Events
{
    public class CodeRepositoryDeploymentStarted : DeploymentEvent
    {
        public CodeRepositoryDeploymentStarted(RepositoryDeployment repositoryDeployment, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            CodeDeployment = true;
            Name = repositoryDeployment.Name;
            Message = $"Code Repository: '{Name}' deployment started.";
            Status = DeploymentStatus.OperationStarted;
        }
    }

    public class CodeRepositoryDeploymentSucceeded: DeploymentEvent
    {
        public CodeRepositoryDeploymentSucceeded(RepositoryDeployment repositoryDeployment, Guid deploymentid)
        {
            DeploymentId = deploymentid;
            Name = repositoryDeployment.Name;
            CodeDeployment = true;
            Id = repositoryDeployment.CloudIdentifier;
            Url = repositoryDeployment.Url;
            Message = $"Code Repository: '{Name}' deployment succeeded.";
            Status = DeploymentStatus.OperationSucceeded;
        }
    }

    public class CodeRepositoryDeploymentFailed : DeploymentEvent
    {
        public CodeRepositoryDeploymentFailed(RepositoryDeployment repositoryDeployment, Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Name = repositoryDeployment.Name;
            CodeDeployment = true;
            Message = $"Code Repository: '{Name}' deployment failed.";
            Error = repositoryDeployment.ErrorMessage;
            Status = DeploymentStatus.OperationFailed;
        }
    }
}
