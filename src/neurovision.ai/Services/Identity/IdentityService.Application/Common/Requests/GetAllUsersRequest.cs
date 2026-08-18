namespace IdentityService.Application.Common.Requests;

public record GetAllUsersRequest(
    [MaxLength(256, ErrorMessage = "Search must be at most 256 characters long.")]
    string? Search) : PaginationRequest;
