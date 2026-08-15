using BuildingBlocks.Pagination;
using Gateway.API.Common.Response;

namespace Gateway.API.Common.Interface
{
    public interface IServiceHealthChecker
    {
        Task<SystemHealthResponse> CheckAsync(PaginationRequest request);
    }
}
