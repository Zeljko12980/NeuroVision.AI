using BuildingBlocks.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Common.Response;
using NotificationService.Application.Feature.Notification.Command.Create;
using NotificationService.Application.Feature.Notification.Command.MarkAsRead;
using NotificationService.Application.Feature.Notification.Query.GetInbox;
using System.Net;

namespace NotificationService.UnitTests.Application.Handlers;

public class NotificationHandlerTests
{
    private readonly INotificationWriteStore _writes = Substitute.For<INotificationWriteStore>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Create_WhenSourceEventExists_ReturnsExistingWithoutInsert()
    {
        var existing = NotificationFactory.Create(sourceEventId: NotificationFactory.DefaultId);
        _writes.FindBySourceEventIdAsync(existing.SourceEventId!.Value, Arg.Any<CancellationToken>())
            .Returns(existing);
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateNotificationCommand(
                existing.RecipientUserId,
                existing.TypeCode,
                existing.SeverityCode,
                existing.Title,
                existing.Message,
                existing.SourceEventId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(existing.Id);
        await _writes.DidNotReceive().AddAsync(Arg.Any<Notification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenNew_PersistsNotification()
    {
        _writes.FindBySourceEventIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Notification?)null);
        var handler = CreateHandler();
        var sourceEventId = Guid.NewGuid();

        var result = await handler.Handle(
            new CreateNotificationCommand(
                NotificationFactory.RecipientId,
                NotificationTypeCodes.System,
                NotificationSeverityCodes.Info,
                "New patient assigned",
                "Haris Delić was assigned to you.",
                sourceEventId,
                RelatedEntityType: "Patient",
                RelatedEntityId: NotificationFactory.RecipientId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        await _writes.Received(1).AddAsync(
            Arg.Is<Notification>(item =>
                item.RecipientUserId == NotificationFactory.RecipientId
                && item.Title == "New patient assigned"
                && item.SourceEventId == sourceEventId),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarkAsRead_WhenMissing_ReturnsNotFound()
    {
        _writes.FindAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Notification?)null);
        var handler = new MarkNotificationAsReadCommandHandler(
            _writes,
            _unitOfWork,
            NullLogger<MarkNotificationAsReadCommandHandler>.Instance);

        var result = await handler.Handle(
            new MarkNotificationAsReadCommand(NotificationFactory.DefaultId, NotificationFactory.RecipientId),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MarkAsRead_WhenFound_SetsReadAt()
    {
        var notification = NotificationFactory.Create();
        _writes.FindAsync(notification.Id, Arg.Any<CancellationToken>()).Returns(notification);
        var handler = new MarkNotificationAsReadCommandHandler(
            _writes,
            _unitOfWork,
            NullLogger<MarkNotificationAsReadCommandHandler>.Instance);

        var result = await handler.Handle(
            new MarkNotificationAsReadCommand(notification.Id, notification.RecipientUserId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsRead.Should().BeTrue();
        notification.ReadAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetInbox_WhenRecipientMissing_ReturnsBadRequest()
    {
        var handler = new GetNotificationInboxQueryHandler(
            _writes,
            NullLogger<GetNotificationInboxQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetNotificationInboxQuery(Guid.Empty),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetInbox_WhenRecipientPresent_ReturnsItemsAndUnreadCount()
    {
        var notification = NotificationFactory.Create();
        _writes.GetInboxAsync(NotificationFactory.RecipientId, 20, Arg.Any<CancellationToken>())
            .Returns(new List<Notification> { notification });
        _writes.CountUnreadAsync(NotificationFactory.RecipientId, Arg.Any<CancellationToken>())
            .Returns(1);
        var handler = new GetNotificationInboxQueryHandler(
            _writes,
            NullLogger<GetNotificationInboxQueryHandler>.Instance);

        var result = await handler.Handle(
            new GetNotificationInboxQuery(NotificationFactory.RecipientId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(item => item.Id == notification.Id);
        result.Value.UnreadCount.Should().Be(1);
    }

    [Fact]
    public async Task Create_WhenNew_PublishesRealtimeEvent()
    {
        _writes.FindBySourceEventIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Notification?)null);
        var realtime = Substitute.For<INotificationRealtimePublisher>();
        var handler = new CreateNotificationCommandHandler(
            _writes,
            _unitOfWork,
            realtime,
            NullLogger<CreateNotificationCommandHandler>.Instance);

        var result = await handler.Handle(
            new CreateNotificationCommand(
                NotificationFactory.RecipientId,
                NotificationTypeCodes.System,
                NotificationSeverityCodes.Info,
                "New patient assigned",
                "Haris Delić was assigned to you.",
                Guid.NewGuid()),
            CancellationToken.None);

        await realtime.Received(1).PublishCreatedAsync(
            Arg.Is<NotificationResponse>(item => item.Id == result.Value.Id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_WhenSourceEventExists_DoesNotPublishRealtimeEvent()
    {
        var existing = NotificationFactory.Create(sourceEventId: NotificationFactory.DefaultId);
        _writes.FindBySourceEventIdAsync(existing.SourceEventId!.Value, Arg.Any<CancellationToken>())
            .Returns(existing);
        var realtime = Substitute.For<INotificationRealtimePublisher>();
        var handler = new CreateNotificationCommandHandler(
            _writes,
            _unitOfWork,
            realtime,
            NullLogger<CreateNotificationCommandHandler>.Instance);

        await handler.Handle(
            new CreateNotificationCommand(
                existing.RecipientUserId,
                existing.TypeCode,
                existing.SeverityCode,
                existing.Title,
                existing.Message,
                existing.SourceEventId),
            CancellationToken.None);

        await realtime.DidNotReceive().PublishCreatedAsync(
            Arg.Any<NotificationResponse>(),
            Arg.Any<CancellationToken>());
    }

    private CreateNotificationCommandHandler CreateHandler() =>
        new(
            _writes,
            _unitOfWork,
            Substitute.For<INotificationRealtimePublisher>(),
            NullLogger<CreateNotificationCommandHandler>.Instance);
}
