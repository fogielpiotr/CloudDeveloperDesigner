using MediatR;

namespace Application.Projects.Command.DeleteProject
{
    public class DeleteProjectCommand : IRequest
    {
        public Guid Id { get; init; }
    }
}
