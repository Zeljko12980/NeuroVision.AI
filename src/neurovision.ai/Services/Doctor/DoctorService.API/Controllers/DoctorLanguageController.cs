using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorLanguage.Command.Create;
using DoctorService.Application.Feature.DoctorLanguage.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorLanguageController : ControllerBase
{
    private readonly ISender sender;

    public DoctorLanguageController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorLanguagesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorLanguagesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorLanguageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorLanguageCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
