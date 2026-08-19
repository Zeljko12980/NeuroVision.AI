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
    public class RegionSettlementCoverageService : IRegionSettlementCoverageService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<RegionSettlementCoverageService> _logger;

        public RegionSettlementCoverageService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<RegionSettlementCoverageService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<RegionSettlementCoverageResponse>> AddAsync(CreateRegionSettlementCoverageRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                RegionSettlementCoverageQueries.Exists,
                new { RegionTypeCode = request.RegionTypeCode, RegionCode = request.RegionCode, CountryCode = request.CountryCode, SettlementCode = request.SettlementCode });

            if (exists > 0)
            {
                return Result<RegionSettlementCoverageResponse>.Fail(
                    "RegionSettlementCoverage already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<RegionSettlementCoverage>();

            await _context.RegionSettlementCoverages.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<RegionSettlementCoverageResponse>.Ok(
                entity.Adapt<RegionSettlementCoverageResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string regionTypeCode, short regionCode, string countryCode, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _context.RegionSettlementCoverages.FindAsync(new object?[] { (object)regionTypeCode, (object)regionCode, (object)countryCode, (object)settlementCode }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "RegionSettlementCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            _context.RegionSettlementCoverages.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<RegionSettlementCoverageResponse>> GetByKeyAsync(string regionTypeCode, short regionCode, string countryCode, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<RegionSettlementCoverage>(
                RegionSettlementCoverageQueries.GetByKey,
                new { RegionTypeCode = regionTypeCode, RegionCode = regionCode, CountryCode = countryCode, SettlementCode = settlementCode });

            if (entity is null)
            {
                return Result<RegionSettlementCoverageResponse>.Fail(
                    "RegionSettlementCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<RegionSettlementCoverageResponse>.Ok(entity.Adapt<RegionSettlementCoverageResponse>());
        }

        public async Task<Result<PaginatedResult<RegionSettlementCoverageResponse>>> GetAllAsync(GetRegionSettlementCoveragesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(RegionSettlementCoverageQueries.Count);

            var items = await _sql.QueryAsync<RegionSettlementCoverage>(
                RegionSettlementCoverageQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<RegionSettlementCoverageResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<RegionSettlementCoverageResponse>>());

            return Result<PaginatedResult<RegionSettlementCoverageResponse>>.Ok(response);
        }
    }
}
