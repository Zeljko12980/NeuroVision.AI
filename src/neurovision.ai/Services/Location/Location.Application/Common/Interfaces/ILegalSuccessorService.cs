using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;

namespace LocationService.Application.Common.Interfaces
{
    public interface ILegalSuccessorService
    {
        Task<Result<LegalSuccessorResponse>> AddAsync(
            CreateLegalSuccessorRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string successorCountryCode, string predecessorCountryCode,
            CancellationToken cancellationToken = default);

        Task<Result<LegalSuccessorResponse>> GetByKeyAsync(
            string successorCountryCode, string predecessorCountryCode,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<LegalSuccessorResponse>>> GetAllAsync(
            GetLegalSuccessorsRequest request,
            CancellationToken cancellationToken = default);
    }
}
