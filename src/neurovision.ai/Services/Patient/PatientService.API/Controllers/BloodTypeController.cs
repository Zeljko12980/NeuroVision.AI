using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.BloodType.Command.Create;
using PatientService.Application.Feature.BloodType.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class BloodTypeController : ControllerBase
{
    private readonly ISender sender;

    public BloodTypeController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetBloodTypesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllBloodTypesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBloodTypeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateBloodTypeCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
