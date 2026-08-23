using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.RegionSettlementCoverage.Command.Create;
using LocationService.Application.Feature.RegionSettlementCoverage.Command.Delete;

using LocationService.Application.Feature.RegionSettlementCoverage.Query.GetAll;
using LocationService.Application.Feature.RegionSettlementCoverage.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class RegionSettlementCoverageController : ControllerBase
    {
        private readonly ISender _sender;

        public RegionSettlementCoverageController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetRegionSettlementCoveragesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllRegionSettlementCoveragesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{regionTypeCode}/{regionCode}/{countryCode}/{settlementCode}")]
        public async Task<IActionResult> GetByKey([FromRoute] string regionTypeCode, [FromRoute] short regionCode, [FromRoute] string countryCode, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var query = new GetRegionSettlementCoverageByKeyQuery(regionTypeCode, regionCode, countryCode, settlementCode);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionSettlementCoverageRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRegionSettlementCoverageCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{regionTypeCode}/{regionCode}/{countryCode}/{settlementCode}")]
        public async Task<IActionResult> Delete([FromRoute] string regionTypeCode, [FromRoute] short regionCode, [FromRoute] string countryCode, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var command = new DeleteRegionSettlementCoverageCommand(regionTypeCode, regionCode, countryCode, settlementCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
