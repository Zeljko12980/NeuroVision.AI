using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorStatus.Command.Create;
using DoctorService.Application.Feature.DoctorStatus.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorStatusController : ControllerBase
{
    private readonly ISender sender;

    public DoctorStatusController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorStatusesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorStatusesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorStatusCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
