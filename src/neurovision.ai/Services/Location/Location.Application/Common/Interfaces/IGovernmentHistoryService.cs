using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IGovernmentHistoryService
    {
        Task<Result<GovernmentHistoryResponse>> AddAsync(
            CreateGovernmentHistoryRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<GovernmentHistoryResponse>> UpdateAsync(
            string countryCode, int sequenceNumber,
            UpdateGovernmentHistoryRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<GovernmentHistoryResponse>> GetByKeyAsync(
            string countryCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<GovernmentHistoryResponse>>> GetAllAsync(
            GetGovernmentHistoriesRequest request,
            CancellationToken cancellationToken = default);
    }
}
