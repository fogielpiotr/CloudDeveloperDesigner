using Domain.Deployments;
using MediatR;

namespace Application.Deployments.Query.GetDeploymentsLogs
{
    public class GetDeploymentLogsQueryHandler : IRequestHandler<GetDeploymentsLogsQuery, List<DeploymentLog>>
    {
        private readonly IDeploymentLogsRepository _repository;

        public GetDeploymentLogsQueryHandler(IDeploymentLogsRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DeploymentLog>> Handle(GetDeploymentsLogsQuery request, CancellationToken cancellationToken)
        {
            var deployments = await _repository.GetAsync(request.DeploymentId, cancellationToken);
            
            return deployments.OrderBy(x => x.Date).ToList();
        }
    }
}
