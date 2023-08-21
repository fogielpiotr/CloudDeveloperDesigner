using MediatR;

namespace Application.Deployments.Command.ExecuteDeployment
{
    public record ExecuteDeploymentCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }
}
