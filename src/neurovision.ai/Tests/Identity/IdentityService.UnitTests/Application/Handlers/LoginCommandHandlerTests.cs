using BuildingBlocks.Messaging.Events;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Requests;
using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class LoginCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _identityService,
            _publishEndpoint,
            NullLogger<LoginCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenCredentialsInvalid_ReturnsUnauthorized()
    {
        _identityService.SignInAsync("user@neurovision.ai", Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(SignInStatus.Failed);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<TwoFactorCodeGeneratedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTwoFactorCodeMissing_ReturnsInternalServerError()
    {
        _identityService.SignInAsync("user@neurovision.ai", "Secret1", Arg.Any<CancellationToken>())
            .Returns(SignInStatus.Succeeded);
        _identityService.GenerateTwoFactorCodeAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns((string?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Handle_WhenLoginSucceeds_PublishesTwoFactorEvent()
    {
        _identityService.SignInAsync("user@neurovision.ai", "Secret1", Arg.Any<CancellationToken>())
            .Returns(SignInStatus.Succeeded);
        _identityService.GenerateTwoFactorCodeAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns("654321");
        _identityService.GetUserNameByEmailAsync("user@neurovision.ai", Arg.Any<CancellationToken>())
            .Returns("doctor.jane");

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be("user@neurovision.ai");
        await _publishEndpoint.Received(1).Publish(
            Arg.Is<TwoFactorCodeGeneratedEvent>(e =>
                e.Email == "user@neurovision.ai"
                && e.Code == "654321"
                && e.FullName == "doctor.jane"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAccountLocked_ReturnsLocked()
    {
        _identityService.SignInAsync("user@neurovision.ai", "Secret1", Arg.Any<CancellationToken>())
            .Returns(SignInStatus.LockedOut);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Locked);
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<TwoFactorCodeGeneratedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEmailNotConfirmed_ReturnsForbidden()
    {
        _identityService.SignInAsync("user@neurovision.ai", "Secret1", Arg.Any<CancellationToken>())
            .Returns(SignInStatus.NotAllowed);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        await _publishEndpoint.DidNotReceive().Publish(Arg.Any<TwoFactorCodeGeneratedEvent>(), Arg.Any<CancellationToken>());
    }

    private static LoginCommand CreateCommand()
        => new(new LoginRequest
        {
            Email = "user@neurovision.ai",
            Password = "Secret1"
        });
}
