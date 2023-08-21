using Application.Projects.Command.AddProject;
using Application.Projects.Command.DeleteProject;
using Application.Projects.Command.EditProject;
using Application.Projects.Queries;
using Application.Projects.Queries.GetProject;
using Application.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(CancellationToken ct)
            => await _mediator.Send(new GetProjectsQuery(), ct);


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id, CancellationToken ct)
            => await _mediator.Send(new GetProjectQuery() { Id = id }, ct);


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProjectDto>> AddProject([FromBody] AddProjectCommand createProjectCommand, CancellationToken ct)
        {
            var result = await _mediator.Send(createProjectCommand, ct);

            return CreatedAtAction(nameof(GetProject), new { id = createProjectCommand.Id }, result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditProject([FromBody] EditProjectCommand editProjectCommand, CancellationToken ct)
        {
            await _mediator.Send(editProjectCommand, ct);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProject(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteProjectCommand() { Id = id }, ct);

            return NoContent();
        }
    }
}
