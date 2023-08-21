using MediatR;
using Domain.Interfaces;
using AutoMapper;

namespace Application.Projects.Queries.GetProjects
{
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectsQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
           var projects = await _projectRepository.GetAllProjectsAsync(cancellationToken);

            return _mapper.Map<List<ProjectDto>>(projects);
        }
    }
}
