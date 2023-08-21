using AutoMapper;
using Domain.Deployments;
using MediatR;

namespace Application.Deployments.Query.GetDeployments
{
    public class GetDeploymentsQueryHandler : IRequestHandler<GetDeploymentsQuery, List<DeploymentDto>>
    {
        private readonly IDeploymentRepository _repository;
        private readonly IMapper _mapper;

        public GetDeploymentsQueryHandler(IDeploymentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<DeploymentDto>> Handle(GetDeploymentsQuery request, CancellationToken cancellationToken)
        {
            var deployments = await _repository.GetAllForProjectAsync(request.ProjectId, cancellationToken);
            var mappedDeployments = _mapper.Map<IEnumerable<DeploymentDto>>(deployments);

            return mappedDeployments.OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
