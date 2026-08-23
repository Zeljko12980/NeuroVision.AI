using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Capital.Command.Create;
using LocationService.Application.Feature.Capital.Command.Delete;
using LocationService.Application.Feature.Capital.Command.Update;
using LocationService.Application.Feature.Capital.Query.GetAll;
using LocationService.Application.Feature.Capital.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class CapitalController : ControllerBase
    {
        private readonly ISender _sender;

        public CapitalController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCapitalsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllCapitalsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{settlementCode}/{sequenceNumber}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int settlementCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var query = new GetCapitalByKeyQuery(countryCode, settlementCode, sequenceNumber);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCapitalRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCapitalCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{countryCode}/{settlementCode}/{sequenceNumber}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int settlementCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var command = new DeleteCapitalCommand(countryCode, settlementCode, sequenceNumber);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPut("{countryCode}/{settlementCode}/{sequenceNumber}")]
        public async Task<IActionResult> Update([FromRoute] string countryCode, [FromRoute] int settlementCode, [FromRoute] int sequenceNumber, [FromBody] UpdateCapitalRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCapitalCommand(request, countryCode, settlementCode, sequenceNumber);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
