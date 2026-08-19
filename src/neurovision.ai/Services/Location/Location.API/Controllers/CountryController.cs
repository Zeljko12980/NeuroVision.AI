using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Country.Command.Create;
using LocationService.Application.Feature.Country.Command.Delete;
using LocationService.Application.Feature.Country.Command.Update;
using LocationService.Application.Feature.Country.Query.GetAll;
using LocationService.Application.Feature.Country.Query.GetByCode;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ISender _sender;

        public CountryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetCountriesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllCountriesQuery(request);

            var result = await _sender.Send(query, cancellationToken);
            

            return result.ToActionResult();
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode([FromRoute] string code, CancellationToken cancellationToken)
        {
            var query = new GetByCodeQuery(code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCountryRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateCountryCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete([FromRoute] string code, CancellationToken cancellationToken)
        {
            var command = new DeleteCountryCommand(code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update([FromRoute] string code, [FromForm] UpdateCountryRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateCountryCommand(request, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
