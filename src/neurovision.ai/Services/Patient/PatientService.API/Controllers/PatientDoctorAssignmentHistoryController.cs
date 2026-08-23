using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientDoctorAssignmentHistory.Command.Create;
using PatientService.Application.Feature.PatientDoctorAssignmentHistory.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientDoctorAssignmentHistoryController : ControllerBase
{
    private readonly ISender sender;

    public PatientDoctorAssignmentHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientDoctorAssignmentHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientDoctorAssignmentHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientDoctorAssignmentHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientDoctorAssignmentHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
