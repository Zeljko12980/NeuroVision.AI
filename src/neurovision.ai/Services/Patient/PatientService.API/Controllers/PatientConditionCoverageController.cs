using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientConditionCoverage.Command.Create;
using PatientService.Application.Feature.PatientConditionCoverage.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class PatientConditionCoverageController : ControllerBase
{
    private readonly ISender sender;

    public PatientConditionCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientConditionCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientConditionCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientConditionCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientConditionCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
