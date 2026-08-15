using BuildingBlocks.Pagination;

namespace Gateway.API.Common.Response
{
    public sealed class SystemHealthResponse
    {
        public string Status { get; set; } = default!;
        public PaginatedResult<ServiceHealthResponse> Services { get; set; } 
    }
}
