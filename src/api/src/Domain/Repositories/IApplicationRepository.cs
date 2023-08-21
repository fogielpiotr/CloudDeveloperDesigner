using Domain.Applications;

namespace Domain.Interfaces
{
    public interface IApplicationRepository
    {
        Task<Domain.Applications.Application> GetApplicationAsync(Guid id, CancellationToken ct);
        public Task<IEnumerable<Domain.Applications.Application>> GetApplicationsAsync(CancellationToken ct);
    }
}
