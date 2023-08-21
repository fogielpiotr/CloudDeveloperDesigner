using MediatR;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries;
using Domain.Interfaces;
using AutoMapper;

namespace Azure.Infrastructure.QueryHandlers
{
    public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public GetProjectQueryHandler(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<ProjectDto> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectAsync(request.Id, cancellationToken);

            return _mapper.Map<ProjectDto>(project);
        }
    }
}
