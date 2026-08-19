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
    public class RegionService : IRegionService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<RegionService> _logger;

        public RegionService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<RegionService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<RegionResponse>> AddAsync(CreateRegionRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                RegionQueries.Exists,
                new { TypeCode = request.TypeCode, Code = request.Code });

            if (exists > 0)
            {
                return Result<RegionResponse>.Fail(
                    "Region already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<Region>();

            await _context.Regions.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<RegionResponse>.Ok(
                entity.Adapt<RegionResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string typeCode, short code, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Regions.FindAsync(new object?[] { (object)typeCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "Region not found.",
                    HttpStatusCode.NotFound);
            }

            _context.Regions.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<RegionResponse>> GetByKeyAsync(string typeCode, short code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<Region>(
                RegionQueries.GetByKey,
                new { TypeCode = typeCode, Code = code });

            if (entity is null)
            {
                return Result<RegionResponse>.Fail(
                    "Region not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<RegionResponse>.Ok(entity.Adapt<RegionResponse>());
        }

        public async Task<Result<PaginatedResult<RegionResponse>>> GetAllAsync(GetRegionsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(RegionQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<Region>(
                RegionQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<RegionResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<RegionResponse>>());

            return Result<PaginatedResult<RegionResponse>>.Ok(response);
        }

        public async Task<Result<RegionResponse>> UpdateAsync(string typeCode, short code, UpdateRegionRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Regions.FindAsync(new object?[] { (object)typeCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<RegionResponse>.Fail(
                    "Region not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.BelongsToCountryCode = request.BelongsToCountryCode;
            entity.HeadquartersCountryCode = request.HeadquartersCountryCode;
            entity.AdministrativeSeatSettlementCode = request.AdministrativeSeatSettlementCode;

            _context.Regions.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<RegionResponse>.Ok(entity.Adapt<RegionResponse>());
        }
    }
}
