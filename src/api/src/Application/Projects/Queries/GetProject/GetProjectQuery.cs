using MediatR;

namespace Application.Projects.Queries.GetProject
{
    public class GetProjectQuery : IRequest<ProjectDto>
    {
        public Guid Id { get; init; }
    }
}
