using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.UnitTests.Application.Handlers;

public class SetPasswordCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly SetPasswordCommandHandler _handler;

    public SetPasswordCommandHandlerTests()
    {
        _handler = new SetPasswordCommandHandler(
            _identityService,
            NullLogger<SetPasswordCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenResetFails_ReturnsFailure()
    {
        _identityService.ResetPasswordAsync("user@neurovision.ai", "token", "Secret1", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(
            new SetPasswordCommand("user@neurovision.ai", "token", "Secret1"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid token or user not found.");
    }

    [Fact]
    public async Task Handle_WhenResetSucceeds_ReturnsOk()
    {
        _identityService.ResetPasswordAsync("user@neurovision.ai", "token", "Secret1", Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(
            new SetPasswordCommand("user@neurovision.ai", "token", "Secret1"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
