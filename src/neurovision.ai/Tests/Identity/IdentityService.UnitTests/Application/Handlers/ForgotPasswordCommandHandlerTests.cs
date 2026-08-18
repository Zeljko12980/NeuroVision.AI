using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Results;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.UnitTests.Application.Handlers;

public class ForgotPasswordCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IFrontendLinkService _frontendLinkService = Substitute.For<IFrontendLinkService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _handler = new ForgotPasswordCommandHandler(
            _identityService,
            _frontendLinkService,
            _publishEndpoint,
            NullLogger<ForgotPasswordCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUserUnknown_ReturnsGenericSuccessWithoutPublishing()
    {
        _identityService.GeneratePasswordResetTokenAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var result = await _handler.Handle(
            new ForgotPasswordCommand("user@neurovision.ai"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Message.Should().Contain("If an account exists");
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<ForgotPasswordEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExists_PublishesForgotPasswordEvent()
    {
        _identityService.GeneratePasswordResetTokenAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns("raw-token");
        _frontendLinkService.BuildSetPasswordLink("user@neurovision.ai", "raw-token")
            .Returns(Result<string>.Ok("http://localhost:5173/set-password?email=user%40neurovision.ai&token=abc"));

        var result = await _handler.Handle(
            new ForgotPasswordCommand("user@neurovision.ai"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<ForgotPasswordEvent>(e =>
                e.Email == "user@neurovision.ai"
                && e.Url.Contains("/set-password")),
            Arg.Any<CancellationToken>());
    }
}
