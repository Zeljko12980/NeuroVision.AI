using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IHealthInstitutionService
    {
        Task<Result<HealthInstitutionResponse>> AddAsync(
            CreateHealthInstitutionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<HealthInstitutionResponse>> UpdateAsync(
            int id,
            UpdateHealthInstitutionRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<HealthInstitutionResponse>> GetByKeyAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<HealthInstitutionResponse>>> GetAllAsync(
            GetHealthInstitutionsRequest request,
            CancellationToken cancellationToken = default);
    }
}
