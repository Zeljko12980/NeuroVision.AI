using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.HealthInstitutionType.Command.Create;
using LocationService.Application.Feature.HealthInstitutionType.Command.Delete;
using LocationService.Application.Feature.HealthInstitutionType.Command.Update;
using LocationService.Application.Feature.HealthInstitutionType.Query.GetAll;
using LocationService.Application.Feature.HealthInstitutionType.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthInstitutionTypeController : ControllerBase
    {
        private readonly ISender _sender;

        public HealthInstitutionTypeController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetHealthInstitutionTypesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllHealthInstitutionTypesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string code, CancellationToken cancellationToken)
        {
            var query = new GetHealthInstitutionTypeByKeyQuery(code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateHealthInstitutionTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateHealthInstitutionTypeCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete([FromRoute] string code, CancellationToken cancellationToken)
        {
            var command = new DeleteHealthInstitutionTypeCommand(code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update([FromRoute] string code, [FromBody] UpdateHealthInstitutionTypeRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateHealthInstitutionTypeCommand(request, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
