using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.HealthInstitution.Command.Create;
using LocationService.Application.Feature.HealthInstitution.Command.Delete;
using LocationService.Application.Feature.HealthInstitution.Command.Update;
using LocationService.Application.Feature.HealthInstitution.Query.GetAll;
using LocationService.Application.Feature.HealthInstitution.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class HealthInstitutionController : ControllerBase
    {
        private readonly ISender _sender;

        public HealthInstitutionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetHealthInstitutionsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllHealthInstitutionsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByKey([FromRoute] int id, CancellationToken cancellationToken)
        {
            var query = new GetHealthInstitutionByKeyQuery(id);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHealthInstitutionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateHealthInstitutionCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var command = new DeleteHealthInstitutionCommand(id);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateHealthInstitutionRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateHealthInstitutionCommand(request, id);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
