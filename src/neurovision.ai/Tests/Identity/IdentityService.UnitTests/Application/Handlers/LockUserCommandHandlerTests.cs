using BuildingBlocks.Results;
using IdentityService.Application.Commands.User;
using IdentityService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class LockUserCommandHandlerTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly LockUserCommandHandler _handler;

    public LockUserCommandHandlerTests()
    {
        _handler = new LockUserCommandHandler(
            _userService,
            _currentUser,
            NullLogger<LockUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenLockingOwnAccount_ReturnsForbidden()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var result = await _handler.Handle(new LockUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        await _userService.DidNotReceive().LockAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(Guid.NewGuid());
        _userService.LockAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Fail("User not found", HttpStatusCode.NotFound));

        var result = await _handler.Handle(new LockUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_WhenLockSucceeds_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(Guid.NewGuid());
        _userService.LockAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var result = await _handler.Handle(new LockUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
