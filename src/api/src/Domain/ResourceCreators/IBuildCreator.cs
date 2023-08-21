using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IBuildCreator
    {
        Task<CloudCreatorResponse<int>> CreateAndQueueBuildForRepository(BuildDeployment buildDeployment, RepositoryDeployment repositoryDeployment, CancellationToken ct);
    }
}
