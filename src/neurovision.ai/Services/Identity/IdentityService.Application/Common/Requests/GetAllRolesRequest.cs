namespace IdentityService.Application.Common.Requests
{
    public record GetAllRolesRequest(string? RoleName) : PaginationRequest;
}
