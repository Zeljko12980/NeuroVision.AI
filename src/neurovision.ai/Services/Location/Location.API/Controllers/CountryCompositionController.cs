using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.CountryComposition.Command.Create;
using LocationService.Application.Feature.CountryComposition.Command.Delete;
using LocationService.Application.Feature.CountryComposition.Command.Update;
using LocationService.Application.Feature.CountryComposition.Query.GetAll;
using LocationService.Application.Feature.CountryComposition.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryCompositionController : ControllerBase
    {
        private readonly ISender _sender;

        public CountryCompositionController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCountryCompositionsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllCountryCompositionsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{unionCountryCode}/{memberCountryCode}/{sequenceNumber}")]
        public async Task<IActionResult> GetByKey([FromRoute] string unionCountryCode, [FromRoute] string memberCountryCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var query = new GetCountryCompositionByKeyQuery(unionCountryCode, memberCountryCode, sequenceNumber);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCountryCompositionRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCountryCompositionCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{unionCountryCode}/{memberCountryCode}/{sequenceNumber}")]
        public async Task<IActionResult> Delete([FromRoute] string unionCountryCode, [FromRoute] string memberCountryCode, [FromRoute] int sequenceNumber, CancellationToken cancellationToken)
        {
            var command = new DeleteCountryCompositionCommand(unionCountryCode, memberCountryCode, sequenceNumber);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{unionCountryCode}/{memberCountryCode}/{sequenceNumber}")]
        public async Task<IActionResult> Update([FromRoute] string unionCountryCode, [FromRoute] string memberCountryCode, [FromRoute] int sequenceNumber, [FromBody] UpdateCountryCompositionRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCountryCompositionCommand(request, unionCountryCode, memberCountryCode, sequenceNumber);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
