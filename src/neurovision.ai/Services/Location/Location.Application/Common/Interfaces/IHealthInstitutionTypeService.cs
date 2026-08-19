using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface IHealthInstitutionTypeService
    {
        Task<Result<HealthInstitutionTypeResponse>> AddAsync(
            CreateHealthInstitutionTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<HealthInstitutionTypeResponse>> UpdateAsync(
            string code,
            UpdateHealthInstitutionTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<HealthInstitutionTypeResponse>> GetByKeyAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<HealthInstitutionTypeResponse>>> GetAllAsync(
            GetHealthInstitutionTypesRequest request,
            CancellationToken cancellationToken = default);
    }
}
