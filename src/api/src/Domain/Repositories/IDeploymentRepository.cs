namespace Domain.Deployments
{
    public interface IDeploymentRepository
    {
        Task AddAsync(Deployment deployment, CancellationToken ct);
        Task<Deployment> GetAsync(Guid deploymentId, CancellationToken ct);
        Task UpdateAsync(Deployment deployment, CancellationToken ct);
        Task<IEnumerable<Deployment>> GetAllForProjectAsync(Guid projectId, CancellationToken ct);
    }
}
