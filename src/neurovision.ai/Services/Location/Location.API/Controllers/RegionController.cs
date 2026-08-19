using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Region.Command.Create;
using LocationService.Application.Feature.Region.Command.Delete;
using LocationService.Application.Feature.Region.Command.Update;
using LocationService.Application.Feature.Region.Query.GetAll;
using LocationService.Application.Feature.Region.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly ISender _sender;

        public RegionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetRegionsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllRegionsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{typeCode}/{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string typeCode, [FromRoute] short code, CancellationToken cancellationToken)
        {
            var query = new GetRegionByKeyQuery(typeCode, code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRegionCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{typeCode}/{code}")]
        public async Task<IActionResult> Delete([FromRoute] string typeCode, [FromRoute] short code, CancellationToken cancellationToken)
        {
            var command = new DeleteRegionCommand(typeCode, code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{typeCode}/{code}")]
        public async Task<IActionResult> Update([FromRoute] string typeCode, [FromRoute] short code, [FromBody] UpdateRegionRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateRegionCommand(request, typeCode, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
