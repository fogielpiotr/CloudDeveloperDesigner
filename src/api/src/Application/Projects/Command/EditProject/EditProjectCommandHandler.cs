using Application.Common.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Projects.Command.EditProject
{
    public class EditProjectCommandHandler : IRequestHandler<EditProjectCommand>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IClock _clock;

        public EditProjectCommandHandler(IProjectRepository projectRepository, ICurrentUserService currentUserService, IClock clock)
        {
            _projectRepository = projectRepository;
            _currentUserService = currentUserService;
            _clock = clock;
        }

        public async Task<Unit> Handle(EditProjectCommand request, CancellationToken cancellationToken)
        {
            var existingProject = await _projectRepository.GetProjectAsync(request.Id, cancellationToken);
            if (existingProject == null)
            {
                throw new ArgumentException("Project not exists");
            }
            if (existingProject.Name != request.Name)
            {
                if (await _projectRepository.ProjectWithNameExistsAsync(request.Name, cancellationToken))
                {
                    throw new ArgumentException($"Project with name: {request.Name} already exists");
                }
            }
            existingProject.Edit(request.Name, request.Description, request.Diagram, _clock.CurrentDate(), await _currentUserService.GetUserName(), request.Environments, request.ResourceGroup, request.Location, request.MandatoryTags);
            await _projectRepository.EditProjectAsync(existingProject, cancellationToken);

            return Unit.Value;
        }
    }
}
