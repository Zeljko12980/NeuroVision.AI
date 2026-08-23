using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorStatusHistory.Command.Create;
using DoctorService.Application.Feature.DoctorStatusHistory.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorStatusHistoryController : ControllerBase
{
    private readonly ISender sender;

    public DoctorStatusHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorStatusHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorStatusHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorStatusHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorStatusHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
