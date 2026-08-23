using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientAffiliationHistory.Command.Create;
using PatientService.Application.Feature.PatientAffiliationHistory.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientAffiliationHistoryController : ControllerBase
{
    private readonly ISender sender;

    public PatientAffiliationHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientAffiliationHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientAffiliationHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientAffiliationHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientAffiliationHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
