using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IRegionSettlementCoverageService
    {
        Task<Result<RegionSettlementCoverageResponse>> AddAsync(
            CreateRegionSettlementCoverageRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string regionTypeCode, short regionCode, string countryCode, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<RegionSettlementCoverageResponse>> GetByKeyAsync(
            string regionTypeCode, short regionCode, string countryCode, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<RegionSettlementCoverageResponse>>> GetAllAsync(
            GetRegionSettlementCoveragesRequest request,
            CancellationToken cancellationToken = default);
    }
}
