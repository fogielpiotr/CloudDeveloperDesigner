using Domain;
using Domain.Interfaces;
using Domain.Projects;
using Infrastructure.Persistance.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly CosmosDbContext _projectDbContext;

        public ProjectRepository(CosmosDbContext projectDbContext)
        {
            _projectDbContext = projectDbContext;
        }

        public async Task<Project> AddProjectAsync(Project project, CancellationToken ct)
        {
            await _projectDbContext.AddAsync(project, ct);
            await _projectDbContext.SaveChangesAsync(ct);

            return project;
        }

        public async Task DeleteProjectAsync(Guid projectId, CancellationToken ct)
        {
            var project = await _projectDbContext.Projects.SingleOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null)
            {
                throw new KeyNotFoundException("Project not exists");
            }
            _projectDbContext.Remove(project);
            await _projectDbContext.SaveChangesAsync(ct);
        }

        public async Task EditProjectAsync(Project project, CancellationToken ct)
        {
            _projectDbContext.Projects.Update(project);
            await _projectDbContext.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct)
        {
            return await _projectDbContext.Projects.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Project> GetProjectAsync(Guid projectId, CancellationToken ct)
        {
            var project = await _projectDbContext.Projects.SingleOrDefaultAsync(x => x.Id == projectId, ct);
            if (project == null)
            {
                throw new KeyNotFoundException("Project not exists");
            }

            return project;
        }

        public async Task<bool> ProjectWithNameExistsAsync(string name, CancellationToken ct)
        {
            var count = await _projectDbContext.Projects.CountAsync(x => x.Name == name, ct);

            return count != 0;
        }
    }
}
