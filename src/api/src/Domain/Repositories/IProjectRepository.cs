using Domain.Projects;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        public Task<Project> AddProjectAsync(Project project, CancellationToken ct);
        public Task EditProjectAsync(Project project, CancellationToken ct);
        public Task DeleteProjectAsync(Guid id, CancellationToken ct);
        public Task<Project> GetProjectAsync(Guid projectId, CancellationToken ct);
        public Task<IEnumerable<Project>> GetAllProjectsAsync(CancellationToken ct);
        public Task<bool> ProjectWithNameExistsAsync(string name, CancellationToken ct);
    }
}
