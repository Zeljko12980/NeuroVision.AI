using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IRegionService
    {
        Task<Result<RegionResponse>> AddAsync(
            CreateRegionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<RegionResponse>> UpdateAsync(
            string typeCode, short code,
            UpdateRegionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string typeCode, short code,
            CancellationToken cancellationToken = default);

        Task<Result<RegionResponse>> GetByKeyAsync(
            string typeCode, short code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<RegionResponse>>> GetAllAsync(
            GetRegionsRequest request,
            CancellationToken cancellationToken = default);
    }
}
