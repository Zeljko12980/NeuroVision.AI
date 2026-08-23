using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DoctorService.Application.Common.Request;
using DoctorService.Application.Feature.DoctorReview.Command.Create;
using DoctorService.Application.Feature.DoctorReview.Query.GetAll;

namespace DoctorService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DoctorReviewController : ControllerBase
{
    private readonly ISender sender;

    public DoctorReviewController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDoctorReviewsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllDoctorReviewsQuery(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateDoctorReviewRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateDoctorReviewCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}
