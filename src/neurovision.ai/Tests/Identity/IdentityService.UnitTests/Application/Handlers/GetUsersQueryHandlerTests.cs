using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using IdentityService.Application.Common.Interfaces;
using IdentityService.Application.Common.Requests;
using IdentityService.Application.Common.Responses;
using IdentityService.Application.Queries.User;
using System.Net;

namespace IdentityService.UnitTests.Application.Handlers;

public class GetUsersQueryHandlerTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _handler = new GetUsersQueryHandler(_userService);
    }

    [Fact]
    public async Task Handle_ReturnsPagedUsers()
    {
        var page = new PaginatedResult<UserResponse>(
            0,
            10,
            1,
            [new UserResponse { UserName = "admin.jane", Email = "jane@neurovision.ai", IsLockedOut = false }]);

        _userService.GetUsersAsync(0, 10, null, Arg.Any<CancellationToken>())
            .Returns(Result<PaginatedResult<UserResponse>>.Ok(page));

        var result = await _handler.Handle(
            new GetUsersQuery(new GetAllUsersRequest(null)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(1);
        result.Value.Data.Should().ContainSingle(user => user.UserName == "admin.jane");
    }
}
