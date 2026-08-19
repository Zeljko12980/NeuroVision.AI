using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.GovernmentType.Command.Create;
using LocationService.Application.Feature.GovernmentType.Command.Delete;
using LocationService.Application.Feature.GovernmentType.Command.Update;
using LocationService.Application.Feature.GovernmentType.Query.GetAll;
using LocationService.Application.Feature.GovernmentType.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentTypeController : ControllerBase
    {
        private readonly ISender _sender;

        public GovernmentTypeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetGovernmentTypesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllGovernmentTypesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string code, CancellationToken cancellationToken)
        {
            var query = new GetGovernmentTypeByKeyQuery(code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGovernmentTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateGovernmentTypeCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete([FromRoute] string code, CancellationToken cancellationToken)
        {
            var command = new DeleteGovernmentTypeCommand(code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update([FromRoute] string code, [FromBody] UpdateGovernmentTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateGovernmentTypeCommand(request, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
