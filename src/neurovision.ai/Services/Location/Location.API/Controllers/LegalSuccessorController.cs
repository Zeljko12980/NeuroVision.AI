using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Feature.LegalSuccessor.Command.Create;
using LocationService.Application.Feature.LegalSuccessor.Command.Delete;

using LocationService.Application.Feature.LegalSuccessor.Query.GetAll;
using LocationService.Application.Feature.LegalSuccessor.Query.GetByKey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LocationService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = AuthPolicies.Staff)]
    public class LegalSuccessorController : ControllerBase
    {
        private readonly ISender _sender;

        public LegalSuccessorController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetLegalSuccessorsRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllLegalSuccessorsQuery(request);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{successorCountryCode}/{predecessorCountryCode}")]
        public async Task<IActionResult> GetByKey([FromRoute] string successorCountryCode, [FromRoute] string predecessorCountryCode, CancellationToken cancellationToken)
        {
            var query = new GetLegalSuccessorByKeyQuery(successorCountryCode, predecessorCountryCode);

            var result = await _sender.Send(query, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLegalSuccessorRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateLegalSuccessorCommand(request);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Policy = AuthPolicies.SuperAdmin)]
        [HttpDelete("{successorCountryCode}/{predecessorCountryCode}")]
        public async Task<IActionResult> Delete([FromRoute] string successorCountryCode, [FromRoute] string predecessorCountryCode, CancellationToken cancellationToken)
        {
            var command = new DeleteLegalSuccessorCommand(successorCountryCode, predecessorCountryCode);

            var result = await _sender.Send(command, cancellationToken);

            return result.ToActionResult();
        }
    }
}
