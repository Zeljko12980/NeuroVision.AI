using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.InsurancePayer.Command.Create;
using PatientService.Application.Feature.InsurancePayer.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class InsurancePayerController : ControllerBase
{
    private readonly ISender sender;

    public InsurancePayerController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetInsurancePayersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllInsurancePayersQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateInsurancePayerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateInsurancePayerCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
