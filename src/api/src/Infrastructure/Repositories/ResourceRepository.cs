using Domain.Interfaces;
using Domain.Resources;
using Infrastructure.Persistance.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly CosmosDbContext _dbContext;

        public ResourceRepository(CosmosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Resource>> GetResourcesAsync(CancellationToken ct)
        {
            var azureResources = await _dbContext.Resources.AsNoTracking().ToListAsync(ct);
            
            return azureResources;
        }
    }
}
