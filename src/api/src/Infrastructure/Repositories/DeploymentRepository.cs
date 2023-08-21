using Infrastructure.Persistance.Cosmos;
using Domain.Deployments;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeploymentRepository : IDeploymentRepository
    {
        private readonly CosmosDbContext _dbContext;

        public DeploymentRepository(CosmosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Deployment> GetAsync(Guid deploymentId, CancellationToken ct)
        {
            var deployment = await _dbContext.Deployments.SingleOrDefaultAsync(x => x.Id == deploymentId, ct);
            if (deployment == null)
            {
                throw new KeyNotFoundException("Deployment not exists");
            }
            return deployment;
        }

        public async Task AddAsync(Deployment deployment, CancellationToken ct)
        {
            await _dbContext.AddAsync(deployment, ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Deployment deployment, CancellationToken ct)
        {
            _dbContext.Deployments.Update(deployment);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Deployment>> GetAllForProjectAsync(Guid projectId, CancellationToken ct)
        {
            var deployments = await _dbContext.Deployments.Where(x => x.ProjectId == projectId).ToListAsync(ct);

            return deployments;
        }
    }
}
