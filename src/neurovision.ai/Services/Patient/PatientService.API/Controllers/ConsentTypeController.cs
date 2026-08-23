using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.ConsentType.Command.Create;
using PatientService.Application.Feature.ConsentType.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class ConsentTypeController : ControllerBase
{
    private readonly ISender sender;

    public ConsentTypeController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetConsentTypesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllConsentTypesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateConsentTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateConsentTypeCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
