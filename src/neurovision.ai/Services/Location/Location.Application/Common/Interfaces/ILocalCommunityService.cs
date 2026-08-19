using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ILocalCommunityService
    {
        Task<Result<LocalCommunityResponse>> AddAsync(
            CreateLocalCommunityRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<LocalCommunityResponse>> UpdateAsync(
            string countryCode, int municipalityCode, int identifier,
            UpdateLocalCommunityRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int municipalityCode, int identifier,
            CancellationToken cancellationToken = default);

        Task<Result<LocalCommunityResponse>> GetByKeyAsync(
            string countryCode, int municipalityCode, int identifier,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<LocalCommunityResponse>>> GetAllAsync(
            GetLocalCommunitiesRequest request,
            CancellationToken cancellationToken = default);
    }
}
