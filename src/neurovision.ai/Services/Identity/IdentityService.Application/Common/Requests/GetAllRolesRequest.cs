namespace IdentityService.Application.Common.Requests;

public record GetAllRolesRequest(
    [MaxLength(50, ErrorMessage = "Role name must be at most 50 characters long.")]
    string? RoleName) : PaginationRequest;
