using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IMunicipalitySettlementCoverageService
    {
        Task<Result<MunicipalitySettlementCoverageResponse>> AddAsync(
            CreateMunicipalitySettlementCoverageRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int municipalityCode, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<MunicipalitySettlementCoverageResponse>> GetByKeyAsync(
            string countryCode, int municipalityCode, int settlementCode,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>> GetAllAsync(
            GetMunicipalitySettlementCoveragesRequest request,
            CancellationToken cancellationToken = default);
    }
}
