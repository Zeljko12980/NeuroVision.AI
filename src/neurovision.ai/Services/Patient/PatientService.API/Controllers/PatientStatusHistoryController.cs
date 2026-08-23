using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientStatusHistory.Command.Create;
using PatientService.Application.Feature.PatientStatusHistory.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientStatusHistoryController : ControllerBase
{
    private readonly ISender sender;

    public PatientStatusHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientStatusHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientStatusHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientStatusHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientStatusHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
