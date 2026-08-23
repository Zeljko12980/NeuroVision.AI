using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.RegionType.Command.Create;
using LocationService.Application.Feature.RegionType.Command.Delete;
using LocationService.Application.Feature.RegionType.Command.Update;
using LocationService.Application.Feature.RegionType.Query.GetAll;
using LocationService.Application.Feature.RegionType.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class RegionTypeController : ControllerBase
    {
        private readonly ISender _sender;

        public RegionTypeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetRegionTypesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllRegionTypesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string code, CancellationToken cancellationToken)
        {
            var query = new GetRegionTypeByKeyQuery(code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRegionTypeCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete([FromRoute] string code, CancellationToken cancellationToken)
        {
            var command = new DeleteRegionTypeCommand(code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPut("{code}")]
        public async Task<IActionResult> Update([FromRoute] string code, [FromBody] UpdateRegionTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateRegionTypeCommand(request, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
