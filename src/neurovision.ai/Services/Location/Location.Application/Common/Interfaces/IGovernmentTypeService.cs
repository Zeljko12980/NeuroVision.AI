using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IGovernmentTypeService
    {
        Task<Result<GovernmentTypeResponse>> AddAsync(
            CreateGovernmentTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<GovernmentTypeResponse>> UpdateAsync(
            string code,
            UpdateGovernmentTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<GovernmentTypeResponse>> GetByKeyAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<GovernmentTypeResponse>>> GetAllAsync(
            GetGovernmentTypesRequest request,
            CancellationToken cancellationToken = default);
    }
}
