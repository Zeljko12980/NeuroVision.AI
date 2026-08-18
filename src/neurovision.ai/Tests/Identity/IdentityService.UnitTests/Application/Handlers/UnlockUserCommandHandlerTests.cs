using BuildingBlocks.Results;
using IdentityService.Application.Commands.User;
using IdentityService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class UnlockUserCommandHandlerTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly UnlockUserCommandHandler _handler;

    public UnlockUserCommandHandlerTests()
    {
        _handler = new UnlockUserCommandHandler(
            _userService,
            NullLogger<UnlockUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        _userService.UnlockAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Fail("User not found", HttpStatusCode.NotFound));

        var result = await _handler.Handle(new UnlockUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUnlockSucceeds_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _userService.UnlockAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var result = await _handler.Handle(new UnlockUserCommand(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
