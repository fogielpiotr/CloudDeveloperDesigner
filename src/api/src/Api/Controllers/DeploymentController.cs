using Application.Deployments.Command.CreateDeployment;
using Application.Deployments.Query;
using Application.Deployments.Query.GetDeployments;
using Application.Deployments.Query.GetDeploymentsLogs;
using Domain.Deployments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeploymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Deploy([FromBody] CreateDeploymentCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);

            return Accepted(command.DeploymentId);
        }


        [HttpGet("{projectId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<DeploymentDto>>> GetDeploymentsForProject(Guid projectId, CancellationToken ct)
            => await _mediator.Send(new GetDeploymentsQuery() { ProjectId = projectId }, ct);


        [HttpGet("logs/{deploymentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<DeploymentLog>>> GetDeploymentsLogs(Guid deploymentId, CancellationToken ct)
            => await _mediator.Send(new GetDeploymentsLogsQuery() { DeploymentId = deploymentId }, ct);

    }
}
