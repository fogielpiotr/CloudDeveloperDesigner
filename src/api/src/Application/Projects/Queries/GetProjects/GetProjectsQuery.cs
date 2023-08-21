using MediatR;

namespace Application.Projects.Queries.GetProjects
{
    public class GetProjectsQuery : IRequest<List<ProjectDto>>
    {
    }
}
