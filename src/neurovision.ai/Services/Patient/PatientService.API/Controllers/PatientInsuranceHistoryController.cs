using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientInsuranceHistory.Command.Create;
using PatientService.Application.Feature.PatientInsuranceHistory.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class PatientInsuranceHistoryController : ControllerBase
{
    private readonly ISender sender;

    public PatientInsuranceHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientInsuranceHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientInsuranceHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientInsuranceHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientInsuranceHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
