using BuildingBlocks.Results;
using IdentityService.Application.Commands.Authentication;
using IdentityService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class UpdateProfileCommandHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly IRoleService _roleService = Substitute.For<IRoleService>();
    private readonly UpdateProfileCommandHandler _handler;

    public UpdateProfileCommandHandlerTests()
    {
        _handler = new UpdateProfileCommandHandler(
            _currentUser,
            _userService,
            _roleService,
            NullLogger<UpdateProfileCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUnauthenticated_ReturnsUnauthorized()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(
            new UpdateProfileCommand("doctor.jane", "+38761111222"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        await _userService.DidNotReceive().UpdateProfileAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUpdateSucceeds_ReturnsUpdatedProfile()
    {
        var userId = Guid.NewGuid();
        var user = User.Restore(
            userId,
            "doctor.jane",
            "jane@neurovision.ai",
            true,
            true,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow,
            "+38761111222");

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _userService.UpdateProfileAsync(userId, "doctor.jane", "+38761111222", Arg.Any<CancellationToken>())
            .Returns(Result<User>.Ok(user));
        _roleService.GetUserRolesAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result<List<string>>.Ok(["Doctor"]));

        var result = await _handler.Handle(
            new UpdateProfileCommand("doctor.jane", "+38761111222"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be("doctor.jane");
        result.Value.PhoneNumber.Should().Be("+38761111222");
        result.Value.Roles.Should().Equal("Doctor");
    }
}
