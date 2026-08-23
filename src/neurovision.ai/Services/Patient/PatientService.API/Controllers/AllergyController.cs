using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Allergy.Command.Create;
using PatientService.Application.Feature.Allergy.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class AllergyController : ControllerBase
{
    private readonly ISender sender;

    public AllergyController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllergiesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllAllergiesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAllergyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateAllergyCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
