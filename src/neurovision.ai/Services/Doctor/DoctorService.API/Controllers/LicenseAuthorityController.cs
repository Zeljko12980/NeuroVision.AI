using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.LicenseAuthority.Command.Create;
using DoctorService.Application.Feature.LicenseAuthority.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.SuperAdmin)]
public class LicenseAuthorityController : ControllerBase
{
    private readonly ISender sender;

    public LicenseAuthorityController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetLicenseAuthoritiesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllLicenseAuthoritiesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLicenseAuthorityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateLicenseAuthorityCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
