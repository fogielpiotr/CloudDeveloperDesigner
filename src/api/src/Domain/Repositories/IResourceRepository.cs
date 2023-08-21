using Domain.Resources;

namespace Domain.Interfaces
{
    public interface IResourceRepository
    {
        public Task<IEnumerable<Resource>> GetResourcesAsync(CancellationToken ct);
    }
}
