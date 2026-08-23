using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Status.Command.Create;
using PatientService.Application.Feature.Status.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class StatusController : ControllerBase
{
    private readonly ISender sender;

    public StatusController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetStatusesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllStatusesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateStatusCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
