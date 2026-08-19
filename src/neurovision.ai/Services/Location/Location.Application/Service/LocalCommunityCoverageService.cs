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
    public class LocalCommunityCoverageService : ILocalCommunityCoverageService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<LocalCommunityCoverageService> _logger;

        public LocalCommunityCoverageService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<LocalCommunityCoverageService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<LocalCommunityCoverageResponse>> AddAsync(CreateLocalCommunityCoverageRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                LocalCommunityCoverageQueries.Exists,
                new { CountryCode = request.CountryCode, MunicipalityCode = request.MunicipalityCode, LocalCommunityIdentifier = request.LocalCommunityIdentifier, SettlementCode = request.SettlementCode });

            if (exists > 0)
            {
                return Result<LocalCommunityCoverageResponse>.Fail(
                    "LocalCommunityCoverage already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<LocalCommunityCoverage>();

            await _context.LocalCommunityCoverages.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<LocalCommunityCoverageResponse>.Ok(
                entity.Adapt<LocalCommunityCoverageResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int municipalityCode, int localCommunityIdentifier, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _context.LocalCommunityCoverages.FindAsync(new object?[] { (object)countryCode, (object)municipalityCode, (object)localCommunityIdentifier, (object)settlementCode }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "LocalCommunityCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            _context.LocalCommunityCoverages.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<LocalCommunityCoverageResponse>> GetByKeyAsync(string countryCode, int municipalityCode, int localCommunityIdentifier, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<LocalCommunityCoverage>(
                LocalCommunityCoverageQueries.GetByKey,
                new { CountryCode = countryCode, MunicipalityCode = municipalityCode, LocalCommunityIdentifier = localCommunityIdentifier, SettlementCode = settlementCode });

            if (entity is null)
            {
                return Result<LocalCommunityCoverageResponse>.Fail(
                    "LocalCommunityCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<LocalCommunityCoverageResponse>.Ok(entity.Adapt<LocalCommunityCoverageResponse>());
        }

        public async Task<Result<PaginatedResult<LocalCommunityCoverageResponse>>> GetAllAsync(GetLocalCommunityCoveragesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(LocalCommunityCoverageQueries.Count);

            var items = await _sql.QueryAsync<LocalCommunityCoverage>(
                LocalCommunityCoverageQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<LocalCommunityCoverageResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<LocalCommunityCoverageResponse>>());

            return Result<PaginatedResult<LocalCommunityCoverageResponse>>.Ok(response);
        }
    }
}
