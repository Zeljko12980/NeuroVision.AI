using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientConsentCoverage.Command.Create;
using PatientService.Application.Feature.PatientConsentCoverage.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class PatientConsentCoverageController : ControllerBase
{
    private readonly ISender sender;

    public PatientConsentCoverageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientConsentCoveragesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientConsentCoveragesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientConsentCoverageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientConsentCoverageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
