using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorLicenseHistory.Command.Create;
using DoctorService.Application.Feature.DoctorLicenseHistory.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorLicenseHistoryController : ControllerBase
{
    private readonly ISender sender;

    public DoctorLicenseHistoryController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorLicenseHistoriesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorLicenseHistoriesQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorLicenseHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorLicenseHistoryCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
