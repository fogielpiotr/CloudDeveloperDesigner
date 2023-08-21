namespace Domain.Deployments
{
    public interface IDeploymentLogsRepository
    {
        Task<IEnumerable<DeploymentLog>> GetAsync(Guid deploymentId, CancellationToken ct);
        Task AddAsync(DeploymentLog deployment, CancellationToken ct);
        void Add(DeploymentLog deploymentLog);
    }
}
