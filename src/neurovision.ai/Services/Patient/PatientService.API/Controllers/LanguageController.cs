using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientService.Application.Common.Request;
using PatientService.Application.Feature.Language.Command.Create;
using PatientService.Application.Feature.Language.Query.GetAll;

namespace PatientService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Policy = AuthPolicies.Staff)]
public class LanguageController : ControllerBase
{
    private readonly ISender sender;

    public LanguageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetLanguagesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllLanguagesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [Authorize(Policy = AuthPolicies.SuperAdmin)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLanguageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateLanguageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
