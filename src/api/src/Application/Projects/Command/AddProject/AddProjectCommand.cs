using Application.Projects.Queries;
using MediatR;

namespace Application.Projects.Command.AddProject
{
    public record AddProjectCommand : IRequest
    {
        public Guid Id = Guid.NewGuid();
        public string Name { get; init; }
        public string Description { get; init; }
    }
}
