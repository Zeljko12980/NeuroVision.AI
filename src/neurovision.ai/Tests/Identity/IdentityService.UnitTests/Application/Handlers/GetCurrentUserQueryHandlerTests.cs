using BuildingBlocks.Results;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Queries.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class GetCurrentUserQueryHandlerTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly IRoleService _roleService = Substitute.For<IRoleService>();
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(
            _currentUser,
            _userService,
            _roleService,
            NullLogger<GetCurrentUserQueryHandler>.Instance);
    }

    [Fact]
    public async Task Handle_WhenUnauthenticated_ReturnsUnauthorized()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        await _userService.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsIdentityFieldsAndRoles()
    {
        var userId = Guid.NewGuid();
        var user = User.Restore(
            userId,
            "doctor.jane",
            "jane@neurovision.ai",
            true,
            true,
            DateTime.UtcNow.AddDays(-1),
            null,
            "+38761111222");

        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(userId);
        _userService.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result<User>.Ok(user));
        _roleService.GetUserRolesAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Result<List<string>>.Ok(["Doctor"]));

        var result = await _handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(userId);
        result.Value.UserName.Should().Be("doctor.jane");
        result.Value.Email.Should().Be("jane@neurovision.ai");
        result.Value.PhoneNumber.Should().Be("+38761111222");
        result.Value.EmailConfirmed.Should().BeTrue();
        result.Value.Roles.Should().Equal("Doctor");
    }
}
