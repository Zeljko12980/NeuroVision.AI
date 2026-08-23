using BuildingBlocks.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Feature.Notification.Command.MarkAllAsRead;
using NotificationService.Application.Feature.Notification.Command.MarkAsRead;
using NotificationService.Application.Feature.Notification.Query.GetInbox;

namespace NotificationService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotificationController : ControllerBase
{
    private readonly ISender sender;

    public NotificationController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetInbox(
        [FromQuery] Guid recipientUserId,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(
            new GetNotificationInboxQuery(recipientUserId, take),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        [FromRoute] Guid id,
        [FromQuery] Guid recipientUserId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new MarkNotificationAsReadCommand(id, recipientUserId),
            cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        [FromQuery] Guid recipientUserId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new MarkAllNotificationsAsReadCommand(recipientUserId),
            cancellationToken);
        return result.ToActionResult();
    }
}
