using BuildingBlocks.Results;
using IdentityService.Application.Commands.User;
using IdentityService.Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using BuildingBlocks.Messaging.Events;

namespace IdentityService.UnitTests.Application.Handlers;

public class DeleteUserCommandHandlerTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _handler = new DeleteUserCommandHandler(
            _userService,
            _currentUser,
            _publishEndpoint,
            NullLogger<DeleteUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenDeletingOwnAccount_ReturnsForbidden()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        await _userService.DidNotReceive().DeleteUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(Guid.NewGuid());
        _userService.DeleteUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Fail("User not found", HttpStatusCode.NotFound));

        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<DeleteUserEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDeleteSucceeds_PublishesEventAndReturnsNoContent()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(Guid.NewGuid());
        _userService.DeleteUserAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var result = await _handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<DeleteUserEvent>(e => e.UserId == userId),
            Arg.Any<CancellationToken>());
    }
}
