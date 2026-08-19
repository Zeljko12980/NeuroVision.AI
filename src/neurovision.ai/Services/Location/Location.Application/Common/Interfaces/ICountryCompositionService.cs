using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ICountryCompositionService
    {
        Task<Result<CountryCompositionResponse>> AddAsync(
            CreateCountryCompositionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<CountryCompositionResponse>> UpdateAsync(
            string unionCountryCode, string memberCountryCode, int sequenceNumber,
            UpdateCountryCompositionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string unionCountryCode, string memberCountryCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<CountryCompositionResponse>> GetByKeyAsync(
            string unionCountryCode, string memberCountryCode, int sequenceNumber,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<CountryCompositionResponse>>> GetAllAsync(
            GetCountryCompositionsRequest request,
            CancellationToken cancellationToken = default);
    }
}
