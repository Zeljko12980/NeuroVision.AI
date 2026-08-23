using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.RegionComposition.Command.Create;
using LocationService.Application.Feature.RegionComposition.Command.Delete;

using LocationService.Application.Feature.RegionComposition.Query.GetAll;
using LocationService.Application.Feature.RegionComposition.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class RegionCompositionController : ControllerBase
    {
        private readonly ISender _sender;

        public RegionCompositionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetRegionCompositionsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllRegionCompositionsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{parentRegionTypeCode}/{parentRegionCode}/{memberRegionTypeCode}/{memberRegionCode}")]
        public async Task<IActionResult> GetByKey([FromRoute] string parentRegionTypeCode, [FromRoute] short parentRegionCode, [FromRoute] string memberRegionTypeCode, [FromRoute] short memberRegionCode, CancellationToken cancellationToken)
        {
            var query = new GetRegionCompositionByKeyQuery(parentRegionTypeCode, parentRegionCode, memberRegionTypeCode, memberRegionCode);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionCompositionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRegionCompositionCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{parentRegionTypeCode}/{parentRegionCode}/{memberRegionTypeCode}/{memberRegionCode}")]
        public async Task<IActionResult> Delete([FromRoute] string parentRegionTypeCode, [FromRoute] short parentRegionCode, [FromRoute] string memberRegionTypeCode, [FromRoute] short memberRegionCode, CancellationToken cancellationToken)
        {
            var command = new DeleteRegionCompositionCommand(parentRegionTypeCode, parentRegionCode, memberRegionTypeCode, memberRegionCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
