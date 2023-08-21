using Domain.Interfaces;
using Infrastructure.Persistance.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class ApplicationRepository : IApplicationRepository
    {
        private readonly CosmosDbContext _dbContext;

        public ApplicationRepository(CosmosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Domain.Applications.Application>> GetApplicationsAsync(CancellationToken ct)
        {
            var azureApplications = await _dbContext.Applications.AsNoTracking().ToListAsync(ct);

            return azureApplications;
        }

        public async Task<Domain.Applications.Application> GetApplicationAsync(Guid id, CancellationToken ct)
        {
            var azureApplication = await _dbContext.Applications.AsNoTracking().FirstAsync(x => x.Id == id, ct);

            return azureApplication;
        }
    }
}