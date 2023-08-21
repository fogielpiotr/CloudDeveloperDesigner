namespace Domain.Deployments
{
    public enum DeploymentStatus
    {
        OperationStarted,
        OperationSucceeded,
        OperationFailed,
        DeploymentFinished,
        DeploymentQueued,
        DeploymentFailed,
        DeploymentStarted,
    }
}
