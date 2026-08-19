using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.LocalCommunityCoverage.Command.Create;
using LocationService.Application.Feature.LocalCommunityCoverage.Command.Delete;

using LocationService.Application.Feature.LocalCommunityCoverage.Query.GetAll;
using LocationService.Application.Feature.LocalCommunityCoverage.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalCommunityCoverageController : ControllerBase
    {
        private readonly ISender _sender;

        public LocalCommunityCoverageController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetLocalCommunityCoveragesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllLocalCommunityCoveragesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{municipalityCode}/{localCommunityIdentifier}/{settlementCode}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int localCommunityIdentifier, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var query = new GetLocalCommunityCoverageByKeyQuery(countryCode, municipalityCode, localCommunityIdentifier, settlementCode);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocalCommunityCoverageRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateLocalCommunityCoverageCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{countryCode}/{municipalityCode}/{localCommunityIdentifier}/{settlementCode}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int localCommunityIdentifier, [FromRoute] int settlementCode, CancellationToken cancellationToken)
        {
            var command = new DeleteLocalCommunityCoverageCommand(countryCode, municipalityCode, localCommunityIdentifier, settlementCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
