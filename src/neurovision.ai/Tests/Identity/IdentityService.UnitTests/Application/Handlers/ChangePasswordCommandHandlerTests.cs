using BuildingBlocks.Results;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class ChangePasswordCommandHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(
            _currentUser,
            _identityService,
            NullLogger<ChangePasswordCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUnauthenticated_ReturnsUnauthorized()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(
            new ChangePasswordCommand("OldPass12", "NewPass12"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        await _identityService.DidNotReceive().ChangePasswordAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCurrentPasswordInvalid_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _identityService.ChangePasswordAsync(userId, "OldPass12", "NewPass12", Arg.Any<CancellationToken>())
            .Returns(Result.Fail("Invalid current password."));

        var result = await _handler.Handle(
            new ChangePasswordCommand("OldPass12", "NewPass12"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Handle_WhenChangeSucceeds_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _identityService.ChangePasswordAsync(userId, "OldPass12", "NewPass12", Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        var result = await _handler.Handle(
            new ChangePasswordCommand("OldPass12", "NewPass12"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
