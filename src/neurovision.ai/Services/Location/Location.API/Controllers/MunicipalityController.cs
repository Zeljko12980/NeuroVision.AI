using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.Municipality.Command.Create;
using LocationService.Application.Feature.Municipality.Command.Delete;
using LocationService.Application.Feature.Municipality.Command.Update;
using LocationService.Application.Feature.Municipality.Query.GetAll;
using LocationService.Application.Feature.Municipality.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class MunicipalityController : ControllerBase
    {
        private readonly ISender _sender;

        public MunicipalityController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetMunicipalitiesRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllMunicipalitiesQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{countryCode}/{code}")]
        public async Task<IActionResult> GetByKey([FromRoute] string countryCode, [FromRoute] int code, CancellationToken cancellationToken)
        {
            var query = new GetMunicipalityByKeyQuery(countryCode, code);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMunicipalityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateMunicipalityCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{countryCode}/{code}")]
        public async Task<IActionResult> Delete([FromRoute] string countryCode, [FromRoute] int code, CancellationToken cancellationToken)
        {
            var command = new DeleteMunicipalityCommand(countryCode, code);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPut("{countryCode}/{code}")]
        public async Task<IActionResult> Update([FromRoute] string countryCode, [FromRoute] int code, [FromBody] UpdateMunicipalityRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateMunicipalityCommand(request, countryCode, code);
            var result = await _sender.Send(command, cancellationToken);
            return result.ToActionResult();
        }
    }
}
