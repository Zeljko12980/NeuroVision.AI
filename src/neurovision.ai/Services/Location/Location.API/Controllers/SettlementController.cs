using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Settlement.Command.Create;
using LocationService.Application.Feature.Settlement.Command.Delete;
using LocationService.Application.Feature.Settlement.Command.Update;
using LocationService.Application.Feature.Settlement.Query.GetAll;
using LocationService.Application.Feature.Settlement.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class SettlementController : ControllerBase
    {
        private readonly ISender _sender;

        public SettlementController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetSettlementsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllSettlementsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int code, CancellationToken cancellationToken)
        {
            var query = new GetSettlementByKeyQuery(countryCode, code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSettlementRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateSettlementCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{countryCode}/{code}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int code, CancellationToken cancellationToken)
        {
            var command = new DeleteSettlementCommand(countryCode, code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPut("{countryCode}/{code}")]
        public async Task<IActionResult> Update([FromRoute] string countryCode, [FromRoute] int code, [FromBody] UpdateSettlementRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateSettlementCommand(request, countryCode, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
