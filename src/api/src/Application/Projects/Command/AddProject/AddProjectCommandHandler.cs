using Application.Common.Interfaces;
using Domain.Projects;
using MediatR;
using Domain.Interfaces;

namespace Application.Projects.Command.AddProject
{
    public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClock _clock;

        public AddProjectCommandHandler(IProjectRepository projectRepository, ICurrentUserService currentUserService, IClock clock)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
            _clock = clock;
        }

        public async Task<Unit> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            if (await _projectRepository.ProjectWithNameExistsAsync(request.Name, cancellationToken))
            {
                throw new ArgumentException($"Project with name: {request.Name} already exists.");
            }
            var project = new Project(
                request.Id, 
                _clock.CurrentDate(), 
                request.Name, 
                await _currentUserService.GetUserName(), 
                request.Description
            );

            await _projectRepository.AddProjectAsync(project, cancellationToken);

            return Unit.Value;
        }
    }
}
