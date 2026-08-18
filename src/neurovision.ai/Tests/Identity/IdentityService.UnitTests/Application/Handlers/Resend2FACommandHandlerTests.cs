using BuildingBlocks.Messaging.Events;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Requests;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.UnitTests.Application.Handlers;

public class Resend2FACommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly Resend2FACommandHandler _handler;

    public Resend2FACommandHandlerTests()
    {
        _handler = new Resend2FACommandHandler(
            _identityService,
            _publishEndpoint,
            NullLogger<Resend2FACommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenCodeCannotBeGenerated_Fails()
    {
        _identityService.GenerateTwoFactorCodeAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<TwoFactorCodeGeneratedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCodeGenerated_PublishesEvent()
    {
        _identityService.GenerateTwoFactorCodeAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns("111222");
        _identityService.GetUserNameByEmailAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Message.Should().Be("New two-factor code sent.");
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<TwoFactorCodeGeneratedEvent>(e =>
                e.Email == "user@neurovision.ai"
                && e.Code == "111222"
                && e.FullName == "user@neurovision.ai"),
            Arg.Any<CancellationToken>());
    }

    private static Resend2FACommand CreateCommand()
        => new(new Resend2FARequest { Email = "user@neurovision.ai" });
}
