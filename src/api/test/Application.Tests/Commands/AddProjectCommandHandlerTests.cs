using Application.Common.Interfaces;
using Application.Projects.Command.AddProject;
using Domain.Interfaces;
using Moq;
using Xunit;
using Shouldly;
using Domain.Projects;

namespace Application.Tests.Commands
{
    public class AddProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepository;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IClock> _clock;
        private readonly DateTimeOffset CurrentTime = new (2022, 12, 20, 0, 0, 0, TimeSpan.Zero);
        private readonly string CurrentUser = "Test";

        private readonly AddProjectCommandHandler _handler;

        public AddProjectCommandHandlerTests()
        {
            _projectRepository = new Mock<IProjectRepository>();

            _currentUserService = new Mock<ICurrentUserService>();
            _currentUserService.Setup(x => x.GetUserName()).ReturnsAsync(CurrentUser);
            
            _clock = new Mock<IClock>();
            _clock.Setup(x => x.CurrentDate()).Returns(CurrentTime);

            _handler = new AddProjectCommandHandler(_projectRepository.Object, _currentUserService.Object, _clock.Object);
        }

        [Fact]
        public async Task Handler_Should_Throws_Exception_When_Project_Name_Exists()
        {
            //Arrange
            _projectRepository.Setup(x => x.ProjectWithNameExistsAsync("Test", default)).ReturnsAsync(true);

            //Act 
            var function = _handler.Handle(new AddProjectCommand() { Description = "desc", Name = "Test" }, default);

            //Assert
            await function.ShouldThrowAsync<ArgumentException>("Project with name: Test already exists");
        }

        [Fact]
        public async Task Handler_Should_Add_Project()
        {
            // Arrange
            _projectRepository.Setup(x => x.ProjectWithNameExistsAsync(It.IsAny<string>(), default)).ReturnsAsync(false);
            var command = new AddProjectCommand { Description = "desc", Name = "Test" };

            // Act
            await _handler.Handle(command, default);

            // Assert
            _projectRepository.Verify(x=>x.AddProjectAsync(It.Is<Project>(p =>
            
                p.Id == command.Id &&
                p.Name == command.Name &&
                p.Description == command.Description &&
                p.CreatedBy == CurrentUser &&
                p.Created == CurrentTime
            ), default), Times.Once());
        }
    }
}
