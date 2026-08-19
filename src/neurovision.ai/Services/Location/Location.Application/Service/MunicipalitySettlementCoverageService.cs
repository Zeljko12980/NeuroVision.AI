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
    public class MunicipalitySettlementCoverageService : IMunicipalitySettlementCoverageService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<MunicipalitySettlementCoverageService> _logger;

        public MunicipalitySettlementCoverageService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<MunicipalitySettlementCoverageService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<MunicipalitySettlementCoverageResponse>> AddAsync(CreateMunicipalitySettlementCoverageRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                MunicipalitySettlementCoverageQueries.Exists,
                new { CountryCode = request.CountryCode, MunicipalityCode = request.MunicipalityCode, SettlementCode = request.SettlementCode });

            if (exists > 0)
            {
                return Result<MunicipalitySettlementCoverageResponse>.Fail(
                    "MunicipalitySettlementCoverage already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<MunicipalitySettlementCoverage>();

            await _context.MunicipalitySettlementCoverages.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<MunicipalitySettlementCoverageResponse>.Ok(
                entity.Adapt<MunicipalitySettlementCoverageResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int municipalityCode, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _context.MunicipalitySettlementCoverages.FindAsync(new object?[] { (object)countryCode, (object)municipalityCode, (object)settlementCode }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "MunicipalitySettlementCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            _context.MunicipalitySettlementCoverages.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<MunicipalitySettlementCoverageResponse>> GetByKeyAsync(string countryCode, int municipalityCode, int settlementCode, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<MunicipalitySettlementCoverage>(
                MunicipalitySettlementCoverageQueries.GetByKey,
                new { CountryCode = countryCode, MunicipalityCode = municipalityCode, SettlementCode = settlementCode });

            if (entity is null)
            {
                return Result<MunicipalitySettlementCoverageResponse>.Fail(
                    "MunicipalitySettlementCoverage not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<MunicipalitySettlementCoverageResponse>.Ok(entity.Adapt<MunicipalitySettlementCoverageResponse>());
        }

        public async Task<Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>> GetAllAsync(GetMunicipalitySettlementCoveragesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(MunicipalitySettlementCoverageQueries.Count);

            var items = await _sql.QueryAsync<MunicipalitySettlementCoverage>(
                MunicipalitySettlementCoverageQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<MunicipalitySettlementCoverageResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<MunicipalitySettlementCoverageResponse>>());

            return Result<PaginatedResult<MunicipalitySettlementCoverageResponse>>.Ok(response);
        }
    }
}
