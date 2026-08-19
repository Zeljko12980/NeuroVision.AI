using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ISettlementService
    {
        Task<Result<SettlementResponse>> AddAsync(
            CreateSettlementRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<SettlementResponse>> UpdateAsync(
            string countryCode, int code,
            UpdateSettlementRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int code,
            CancellationToken cancellationToken = default);

        Task<Result<SettlementResponse>> GetByKeyAsync(
            string countryCode, int code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<SettlementResponse>>> GetAllAsync(
            GetSettlementsRequest request,
            CancellationToken cancellationToken = default);
    }
}
