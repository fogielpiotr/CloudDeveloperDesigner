using Domain.Deployments;
using MediatR;

namespace Application.Deployments.Query.GetDeployments 
{ 
    public class GetDeploymentsQuery : IRequest<List<DeploymentDto>>
    {
        public Guid ProjectId { get; init; }
    }
}
