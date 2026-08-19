using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.GovernmentHistory.Command.Create;
using LocationService.Application.Feature.GovernmentHistory.Command.Delete;
using LocationService.Application.Feature.GovernmentHistory.Command.Update;
using LocationService.Application.Feature.GovernmentHistory.Query.GetAll;
using LocationService.Application.Feature.GovernmentHistory.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GovernmentHistoryController : ControllerBase
    {
        private readonly ISender _sender;

        public GovernmentHistoryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetGovernmentHistoriesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllGovernmentHistoriesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{sequenceNumber}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var query = new GetGovernmentHistoryByKeyQuery(countryCode, sequenceNumber);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGovernmentHistoryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateGovernmentHistoryCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{countryCode}/{sequenceNumber}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var command = new DeleteGovernmentHistoryCommand(countryCode, sequenceNumber);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{countryCode}/{sequenceNumber}")]
        public async Task<IActionResult> Update([FromRoute] string countryCode, [FromRoute] int sequenceNumber, [FromBody] UpdateGovernmentHistoryRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateGovernmentHistoryCommand(request, countryCode, sequenceNumber);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
