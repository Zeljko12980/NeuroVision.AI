using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IRegionCompositionService
    {
        Task<Result<RegionCompositionResponse>> AddAsync(
            CreateRegionCompositionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string parentRegionTypeCode, short parentRegionCode, string memberRegionTypeCode, short memberRegionCode,
            CancellationToken cancellationToken = default);

        Task<Result<RegionCompositionResponse>> GetByKeyAsync(
            string parentRegionTypeCode, short parentRegionCode, string memberRegionTypeCode, short memberRegionCode,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<RegionCompositionResponse>>> GetAllAsync(
            GetRegionCompositionsRequest request,
            CancellationToken cancellationToken = default);
    }
}
