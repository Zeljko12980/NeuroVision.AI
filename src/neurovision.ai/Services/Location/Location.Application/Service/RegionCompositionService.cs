using BuildingBlocks.Dapper;
using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Interfaces;
using LocationService.Application.Common.Queries;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;
using LocationService.Domain.Entities;
using Mapster;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LocationService.Application.Service
{
    public class RegionCompositionService : IRegionCompositionService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<RegionCompositionService> _logger;

        public RegionCompositionService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<RegionCompositionService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<RegionCompositionResponse>> AddAsync(CreateRegionCompositionRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                RegionCompositionQueries.Exists,
                new { ParentRegionTypeCode = request.ParentRegionTypeCode, ParentRegionCode = request.ParentRegionCode, MemberRegionTypeCode = request.MemberRegionTypeCode, MemberRegionCode = request.MemberRegionCode });

            if (exists > 0)
            {
                return Result<RegionCompositionResponse>.Fail(
                    "RegionComposition already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<RegionComposition>();

            await _context.RegionCompositions.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<RegionCompositionResponse>.Ok(
                entity.Adapt<RegionCompositionResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string parentRegionTypeCode, short parentRegionCode, string memberRegionTypeCode, short memberRegionCode, CancellationToken cancellationToken = default)
        {
            var entity = await _context.RegionCompositions.FindAsync(new object?[] { (object)parentRegionTypeCode, (object)parentRegionCode, (object)memberRegionTypeCode, (object)memberRegionCode }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "RegionComposition not found.",
                    HttpStatusCode.NotFound);
            }

            _context.RegionCompositions.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<RegionCompositionResponse>> GetByKeyAsync(string parentRegionTypeCode, short parentRegionCode, string memberRegionTypeCode, short memberRegionCode, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<RegionComposition>(
                RegionCompositionQueries.GetByKey,
                new { ParentRegionTypeCode = parentRegionTypeCode, ParentRegionCode = parentRegionCode, MemberRegionTypeCode = memberRegionTypeCode, MemberRegionCode = memberRegionCode });

            if (entity is null)
            {
                return Result<RegionCompositionResponse>.Fail(
                    "RegionComposition not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<RegionCompositionResponse>.Ok(entity.Adapt<RegionCompositionResponse>());
        }

        public async Task<Result<PaginatedResult<RegionCompositionResponse>>> GetAllAsync(GetRegionCompositionsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(RegionCompositionQueries.Count);

            var items = await _sql.QueryAsync<RegionComposition>(
                RegionCompositionQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<RegionCompositionResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<RegionCompositionResponse>>());

            return Result<PaginatedResult<RegionCompositionResponse>>.Ok(response);
        }
    }
}
