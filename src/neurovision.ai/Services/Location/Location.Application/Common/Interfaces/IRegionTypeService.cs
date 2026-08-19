using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IRegionTypeService
    {
        Task<Result<RegionTypeResponse>> AddAsync(
            CreateRegionTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<RegionTypeResponse>> UpdateAsync(
            string code,
            UpdateRegionTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<RegionTypeResponse>> GetByKeyAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<RegionTypeResponse>>> GetAllAsync(
            GetRegionTypesRequest request,
            CancellationToken cancellationToken = default);
    }
}
