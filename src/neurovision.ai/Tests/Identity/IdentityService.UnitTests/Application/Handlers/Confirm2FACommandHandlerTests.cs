using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Requests;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.UnitTests.Application.Handlers;

public class Confirm2FACommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly IJwtTokenGenerator _jwt = Substitute.For<IJwtTokenGenerator>();
    private readonly Confirm2FACommandHandler _handler;

    public Confirm2FACommandHandlerTests()
    {
        _handler = new Confirm2FACommandHandler(
            _identityService,
            _userService,
            _jwt,
            NullLogger<Confirm2FACommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenCodeInvalid_Fails()
    {
        _identityService.VerifyTwoFactorAsync("user@neurovision.ai", "000000", Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(CreateCommand("000000"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid or expired 2FA code.");
        _jwt.DidNotReceive().GenerateToken(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<IList<string>>());
    }

    [Fact]
    public async Task Handle_WhenVerified_ReturnsJwt()
    {
        var userId = Guid.NewGuid();
        var user = User.Create(userId, "doctor.jane", "user@neurovision.ai");

        _identityService.VerifyTwoFactorAsync("user@neurovision.ai", "123456", Arg.Any<CancellationToken>())
            .Returns(true);
        _identityService.GetUserRolesAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns(new List<string> { RoleNames.Doctor });
        _userService.GetByEmailAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns(Result<User>.Ok(user));
        _jwt.GenerateToken(userId, "user@neurovision.ai", "doctor.jane", Arg.Any<IList<string>>())
            .Returns("jwt-token");

        var result = await _handler.Handle(CreateCommand("123456"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("jwt-token");
        result.Value.Message.Should().Be("Login successful.");
    }

    private static Confirm2FACommand CreateCommand(string code)
        => new(new Confirm2FARequest
        {
            Email = "user@neurovision.ai",
            Code = code
        });
}
