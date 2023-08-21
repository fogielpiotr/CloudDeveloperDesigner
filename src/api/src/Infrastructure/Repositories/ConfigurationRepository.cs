using Domain;
using Domain.Interfaces;
using Infrastructure.Persistance.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class ConfigurationRepository : IConfigurationRepository
    {
        private readonly CosmosDbContext _dbContext;

        public ConfigurationRepository(CosmosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Configuration> GetConfigurationAsync(CancellationToken ct)
        {
            return _dbContext.Configurations.FirstOrDefaultAsync(ct);
        }
    }
}