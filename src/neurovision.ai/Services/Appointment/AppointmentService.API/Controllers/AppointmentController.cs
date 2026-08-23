using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AppointmentService.Application.Common.Response;
using AppointmentService.Application.Feature.Appointment.Command.Cancel;
using AppointmentService.Application.Feature.Appointment.Command.Create;
using AppointmentService.Application.Feature.Appointment.Command.Reschedule;
using AppointmentService.Application.Feature.Appointment.Query.GetById;
using AppointmentService.Application.Feature.Appointment.Query.GetCatalogs;
using AppointmentService.Application.Feature.Appointment.Query.GetRange;

namespace AppointmentService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AppointmentController : ControllerBase
{
    private readonly ISender sender;

    public AppointmentController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetRange(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] Guid? patientId,
        [FromQuery] Guid? doctorId,
        CancellationToken cancellationToken)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await sender.Send(
            new GetAppointmentRangeQuery(from, to, actor, patientId, doctorId),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("catalogs")]
    public async Task<IActionResult> GetCatalogs(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAppointmentCatalogsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await sender.Send(new GetAppointmentByIdQuery(id, actor), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await sender.Send(new CreateAppointmentCommand(request, actor), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Reschedule(
        [FromRoute] Guid id,
        [FromBody] RescheduleAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await sender.Send(
            new RescheduleAppointmentCommand(id, request, actor),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (this.RequireActor(out var actor) is { } unauthorized)
            return unauthorized;

        var result = await sender.Send(new CancelAppointmentCommand(id, actor), cancellationToken);
        return result.ToActionResult();
    }
}
