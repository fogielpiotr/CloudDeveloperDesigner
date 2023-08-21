using Domain.Deployments;
using MediatR;

namespace Application.Deployments.Query.GetDeploymentsLogs
{
    public class GetDeploymentsLogsQuery : IRequest<List<DeploymentLog>>
    {
        public Guid DeploymentId { get; init; }
    }
}
