using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.LocalCommunity.Command.Create;
using LocationService.Application.Feature.LocalCommunity.Command.Delete;
using LocationService.Application.Feature.LocalCommunity.Command.Update;
using LocationService.Application.Feature.LocalCommunity.Query.GetAll;
using LocationService.Application.Feature.LocalCommunity.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalCommunityController : ControllerBase
    {
        private readonly ISender _sender;

        public LocalCommunityController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetLocalCommunitiesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllLocalCommunitiesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{municipalityCode}/{identifier}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int identifier, CancellationToken cancellationToken)
        {
            var query = new GetLocalCommunityByKeyQuery(countryCode, municipalityCode, identifier);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocalCommunityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateLocalCommunityCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{countryCode}/{municipalityCode}/{identifier}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int identifier, CancellationToken cancellationToken)
        {
            var command = new DeleteLocalCommunityCommand(countryCode, municipalityCode, identifier);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{countryCode}/{municipalityCode}/{identifier}")]
        public async Task<IActionResult> Update([FromRoute] string countryCode, [FromRoute] int municipalityCode, [FromRoute] int identifier, [FromBody] UpdateLocalCommunityRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateLocalCommunityCommand(request, countryCode, municipalityCode, identifier);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
