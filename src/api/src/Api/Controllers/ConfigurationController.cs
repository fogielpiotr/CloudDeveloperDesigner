using Application.Configuration.Query.GetConfigurations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigurationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<Domain.Configuration>> GetConfigurations(CancellationToken ct)
            => await _mediator.Send(new GetConfigurationsQuery(), ct);
    }
}
