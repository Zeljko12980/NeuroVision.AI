using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Create;
using LocationService.Application.Feature.MunicipalitySettlementCoverage.Command.Delete;

using LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetAll;
using LocationService.Application.Feature.MunicipalitySettlementCoverage.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class MunicipalitySettlementCoverageController : ControllerBase
    {
        private readonly ISender _sender;

        public MunicipalitySettlementCoverageController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetMunicipalitySettlementCoveragesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllMunicipalitySettlementCoveragesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{municipalityCode}/{settlementCode}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var query = new GetMunicipalitySettlementCoverageByKeyQuery(countryCode, municipalityCode, settlementCode);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMunicipalitySettlementCoverageRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateMunicipalitySettlementCoverageCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{countryCode}/{municipalityCode}/{settlementCode}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var command = new DeleteMunicipalitySettlementCoverageCommand(countryCode, municipalityCode, settlementCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
