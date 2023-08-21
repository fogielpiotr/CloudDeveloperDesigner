using Domain.Deployments;

namespace Domain.Events
{
    public class DeploymentQueued : DeploymentEvent
    {
        public DeploymentQueued(Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Message = $"Deployment with identifier: '{DeploymentId}' queued.";
            Name = deploymentId.ToString();
            Status = DeploymentStatus.DeploymentQueued;
        }
    }

    public class DeploymentStarted: DeploymentEvent
    {
        public DeploymentStarted(Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Message = $"Deployment started.";
            Name = deploymentId.ToString();
            Status = DeploymentStatus.DeploymentStarted;
        }
    }

    public class DeploymentFinished: DeploymentEvent
    {
        public DeploymentFinished(Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Message = $"Deployment finished.";
            Name = deploymentId.ToString();
            Status = DeploymentStatus.DeploymentFinished;
        }
    }

    public class DeploymentNotFound: DeploymentEvent
    {
        public DeploymentNotFound(Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Message = $"Deployment not found.";
            Name = deploymentId.ToString();
            Status = DeploymentStatus.DeploymentFailed;
        }
    }

    public class DeploymentFailed: DeploymentEvent
    {
        public DeploymentFailed(Guid deploymentId)
        {
            DeploymentId = deploymentId;
            Message = $"An error occurs when processing Deployment with identifier: '{DeploymentId}'.";
            Name = deploymentId.ToString();
            Status = DeploymentStatus.DeploymentFailed;
        }
    }
}
