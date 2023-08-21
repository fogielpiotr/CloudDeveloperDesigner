using Domain.Deployments;

namespace Domain.Interfaces
{
    public interface IRemoteCodeRepositoryCreator
    {
        Task<CloudCreatorResponse<Guid>> CreateRemoteCodeRepositoryAsync(RepositoryDeployment codeDeployment, CancellationToken ct);
    }
}
