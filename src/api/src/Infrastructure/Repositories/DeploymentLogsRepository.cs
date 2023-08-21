using Infrastructure.Persistance.Cosmos;
using Domain.Deployments;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeploymentLogsRepository : IDeploymentLogsRepository
    {
        private readonly CosmosDbContext _dbContext;
        private readonly IDbContextFactory<CosmosDbContext> _contextFactory;

        public DeploymentLogsRepository(CosmosDbContext dbContext, IDbContextFactory<CosmosDbContext> contextFactory)
        {
            _dbContext = dbContext;
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<DeploymentLog>> GetAsync(Guid deploymentId, CancellationToken ct)
        {
            var deploymentLogs = await _dbContext.DeploymentLogs.Where(x => x.DeploymentId == deploymentId).ToListAsync(ct);

            return deploymentLogs;
        }

        public async Task AddAsync(DeploymentLog deploymentLogs, CancellationToken ct)
        {
            using (var transientDbContext = await _contextFactory.CreateDbContextAsync(ct))
            {
                await transientDbContext.AddAsync(deploymentLogs, ct);
                await transientDbContext.SaveChangesAsync(ct);
            }
        }

        public void Add(DeploymentLog deploymentLog)
        {
            _dbContext.Add(deploymentLog);
            _dbContext.SaveChanges();
        }
    }
}
