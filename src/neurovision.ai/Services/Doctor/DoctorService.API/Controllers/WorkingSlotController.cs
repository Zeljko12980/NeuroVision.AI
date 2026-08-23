using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.WorkingSlot.Command.Create;
using DoctorService.Application.Feature.WorkingSlot.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class WorkingSlotController : ControllerBase
{
    private readonly ISender sender;

    public WorkingSlotController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetWorkingSlotsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllWorkingSlotsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateWorkingSlotRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateWorkingSlotCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
