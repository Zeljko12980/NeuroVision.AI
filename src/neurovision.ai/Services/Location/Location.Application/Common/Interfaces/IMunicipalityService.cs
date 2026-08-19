using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IMunicipalityService
    {
        Task<Result<MunicipalityResponse>> AddAsync(
            CreateMunicipalityRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<MunicipalityResponse>> UpdateAsync(
            string countryCode, int code,
            UpdateMunicipalityRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string countryCode, int code,
            CancellationToken cancellationToken = default);

        Task<Result<MunicipalityResponse>> GetByKeyAsync(
            string countryCode, int code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<MunicipalityResponse>>> GetAllAsync(
            GetMunicipalitiesRequest request,
            CancellationToken cancellationToken = default);
    }
}
