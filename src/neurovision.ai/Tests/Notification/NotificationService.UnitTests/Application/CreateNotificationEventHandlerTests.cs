using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Results;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationService.Application.EventHandlers;
using NotificationService.Application.Feature.Notification.Command.Create;
using System.Net;

namespace NotificationService.UnitTests.Application;

public class CreateNotificationEventHandlerTests
{
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly CreateNotificationEventHandler _handler;

    public CreateNotificationEventHandlerTests()
    {
        _handler = new CreateNotificationEventHandler(
            _sender,
            NullLogger<CreateNotificationEventHandler>.Instance);
    }

    [Fact]
    public async Task Consume_MapsEventToCreateCommand()
    {
        var recipientId = Guid.NewGuid();
        var sourceEventId = Guid.NewGuid();
        _sender.Send(Arg.Any<CreateNotificationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<NotificationService.Application.Common.Response.NotificationResponse>.Created(
                new NotificationService.Application.Common.Response.NotificationResponse { Id = Guid.NewGuid() }));

        var context = Substitute.For<ConsumeContext<CreateNotificationEvent>>();
        context.Message.Returns(new CreateNotificationEvent(
            recipientId,
            NotificationTypeCodes.System,
            NotificationSeverityCodes.Info,
            "New patient assigned",
            "Haris Delić was assigned to you.",
            sourceEventId,
            RelatedEntityType: "Patient",
            RelatedEntityId: recipientId));
        context.CancellationToken.Returns(CancellationToken.None);

        await _handler.Consume(context);

        await _sender.Received(1).Send(
            Arg.Is<CreateNotificationCommand>(command =>
                command.RecipientUserId == recipientId
                && command.Title == "New patient assigned"
                && command.SourceEventId == sourceEventId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WhenCreateFails_Throws()
    {
        _sender.Send(Arg.Any<CreateNotificationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<NotificationService.Application.Common.Response.NotificationResponse>.Fail(
                "db down",
                HttpStatusCode.InternalServerError));

        var context = Substitute.For<ConsumeContext<CreateNotificationEvent>>();
        context.Message.Returns(new CreateNotificationEvent(
            Guid.NewGuid(),
            NotificationTypeCodes.System,
            NotificationSeverityCodes.Info,
            "Title",
            "Message",
            Guid.NewGuid()));
        context.CancellationToken.Returns(CancellationToken.None);

        var act = () => _handler.Consume(context);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
