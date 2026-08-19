using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ILocalCommunityCoverageService
    {
        Task<Result<LocalCommunityCoverageResponse>> AddAsync(
            CreateLocalCommunityCoverageRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int municipalityCode, int localCommunityIdentifier, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<LocalCommunityCoverageResponse>> GetByKeyAsync(
            string countryCode, int municipalityCode, int localCommunityIdentifier, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<LocalCommunityCoverageResponse>>> GetAllAsync(
            GetLocalCommunityCoveragesRequest request,
            CancellationToken cancellationToken = default);
    }
}
