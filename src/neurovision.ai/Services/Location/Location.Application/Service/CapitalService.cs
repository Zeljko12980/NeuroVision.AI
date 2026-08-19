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
    public class CapitalService : ICapitalService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<CapitalService> _logger;

        public CapitalService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<CapitalService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<CapitalResponse>> AddAsync(CreateCapitalRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                CapitalQueries.Exists,
                new { CountryCode = request.CountryCode, SettlementCode = request.SettlementCode, SequenceNumber = request.SequenceNumber });

            if (exists > 0)
            {
                return Result<CapitalResponse>.Fail(
                    "Capital already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<Capital>();

            await _context.Capitals.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CapitalResponse>.Ok(
                entity.Adapt<CapitalResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int settlementCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Capitals.FindAsync(new object?[] { (object)countryCode, (object)settlementCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "Capital not found.",
                    HttpStatusCode.NotFound);
            }

            _context.Capitals.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<CapitalResponse>> GetByKeyAsync(string countryCode, int settlementCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<Capital>(
                CapitalQueries.GetByKey,
                new { CountryCode = countryCode, SettlementCode = settlementCode, SequenceNumber = sequenceNumber });

            if (entity is null)
            {
                return Result<CapitalResponse>.Fail(
                    "Capital not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<CapitalResponse>.Ok(entity.Adapt<CapitalResponse>());
        }

        public async Task<Result<PaginatedResult<CapitalResponse>>> GetAllAsync(GetCapitalsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(CapitalQueries.Count);

            var items = await _sql.QueryAsync<Capital>(
                CapitalQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<CapitalResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<CapitalResponse>>());

            return Result<PaginatedResult<CapitalResponse>>.Ok(response);
        }

        public async Task<Result<CapitalResponse>> UpdateAsync(string countryCode, int settlementCode, int sequenceNumber, UpdateCapitalRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Capitals.FindAsync(new object?[] { (object)countryCode, (object)settlementCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<CapitalResponse>.Fail(
                    "Capital not found.",
                    HttpStatusCode.NotFound);
            }

            entity.From = request.From;
            entity.To = request.To;

            _context.Capitals.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CapitalResponse>.Ok(entity.Adapt<CapitalResponse>());
        }
    }
}
