using IdentityService.Application.Commands.Role;
using IdentityService.Application.Common.Interfaces;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class CreateRoleCommandHandlerTests
{
    private readonly IRoleService _roleService = Substitute.For<IRoleService>();
    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _handler = new CreateRoleCommandHandler(_roleService);
    }

    [Fact]
    public async Task Handle_WhenRoleCreated_MapsToResponse()
    {
        var role = Role.Create(Guid.NewGuid(), RoleNames.Doctor, "Medical professional");
        _roleService.CreateRoleAsync(RoleNames.Doctor, "Medical professional", Arg.Any<CancellationToken>())
            .Returns(Result<Role>.Created(role));

        var result = await _handler.Handle(
            new CreateRoleCommand(RoleNames.Doctor, "Medical professional"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Value.Id.Should().Be(role.Id);
        result.Value.Name.Should().Be(RoleNames.Doctor);
        result.Value.Description.Should().Be("Medical professional");
    }

    [Fact]
    public async Task Handle_WhenRoleExists_PropagatesFailure()
    {
        _roleService.CreateRoleAsync(RoleNames.Doctor, null, Arg.Any<CancellationToken>())
            .Returns(Result<Role>.Fail("Role already exists", HttpStatusCode.Conflict));

        var result = await _handler.Handle(
            new CreateRoleCommand(RoleNames.Doctor, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Should().Be("Role already exists");
    }
}
