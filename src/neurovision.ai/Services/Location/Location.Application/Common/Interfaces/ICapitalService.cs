using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ICapitalService
    {
        Task<Result<CapitalResponse>> AddAsync(
            CreateCapitalRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<CapitalResponse>> UpdateAsync(
            string countryCode, int settlementCode, int sequenceNumber,
            UpdateCapitalRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int settlementCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<CapitalResponse>> GetByKeyAsync(
            string countryCode, int settlementCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<CapitalResponse>>> GetAllAsync(
            GetCapitalsRequest request,
            CancellationToken cancellationToken = default);
    }
}
