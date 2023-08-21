using Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Application.Applications.Queries.GetApplications
{
    public class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, List<ApplicationDto>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationsQueryHandler(IMapper mapper, IApplicationRepository azureApplicationRepository)
        {
            _mapper = mapper;
            _applicationRepository = azureApplicationRepository;
        }

        public async Task<List<ApplicationDto>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
        {
            var applications = await _applicationRepository.GetApplicationsAsync(cancellationToken);
            var mapped = _mapper.Map<List<ApplicationDto>>(applications);

            return mapped;
        }
    }
}
