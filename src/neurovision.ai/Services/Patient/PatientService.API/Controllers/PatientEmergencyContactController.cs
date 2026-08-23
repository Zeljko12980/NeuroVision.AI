using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.PatientEmergencyContact.Command.Create;
using PatientService.Application.Feature.PatientEmergencyContact.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class PatientEmergencyContactController : ControllerBase
{
    private readonly ISender sender;

    public PatientEmergencyContactController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPatientEmergencyContactsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllPatientEmergencyContactsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePatientEmergencyContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreatePatientEmergencyContactCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
