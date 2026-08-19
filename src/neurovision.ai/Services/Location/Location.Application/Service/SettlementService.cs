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
    public class SettlementService : ISettlementService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<SettlementService> _logger;

        public SettlementService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<SettlementService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<SettlementResponse>> AddAsync(CreateSettlementRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                SettlementQueries.Exists,
                new { CountryCode = request.CountryCode, Code = request.Code });

            if (exists > 0)
            {
                return Result<SettlementResponse>.Fail(
                    "Settlement already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<Settlement>();

            await _context.Settlements.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<SettlementResponse>.Ok(
                entity.Adapt<SettlementResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int code, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Settlements.FindAsync(new object?[] { (object)countryCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "Settlement not found.",
                    HttpStatusCode.NotFound);
            }

            _context.Settlements.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<SettlementResponse>> GetByKeyAsync(string countryCode, int code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<Settlement>(
                SettlementQueries.GetByKey,
                new { CountryCode = countryCode, Code = code });

            if (entity is null)
            {
                return Result<SettlementResponse>.Fail(
                    "Settlement not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<SettlementResponse>.Ok(entity.Adapt<SettlementResponse>());
        }

        public async Task<Result<PaginatedResult<SettlementResponse>>> GetAllAsync(GetSettlementsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(SettlementQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<Settlement>(
                SettlementQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<SettlementResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<SettlementResponse>>());

            return Result<PaginatedResult<SettlementResponse>>.Ok(response);
        }

        public async Task<Result<SettlementResponse>> UpdateAsync(string countryCode, int code, UpdateSettlementRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Settlements.FindAsync(new object?[] { (object)countryCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<SettlementResponse>.Fail(
                    "Settlement not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.PostalCode = request.PostalCode;

            _context.Settlements.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<SettlementResponse>.Ok(entity.Adapt<SettlementResponse>());
        }
    }
}
