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
    public class GovernmentHistoryService : IGovernmentHistoryService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<GovernmentHistoryService> _logger;

        public GovernmentHistoryService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<GovernmentHistoryService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<GovernmentHistoryResponse>> AddAsync(CreateGovernmentHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                GovernmentHistoryQueries.Exists,
                new { CountryCode = request.CountryCode, SequenceNumber = request.SequenceNumber });

            if (exists > 0)
            {
                return Result<GovernmentHistoryResponse>.Fail(
                    "GovernmentHistory already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<GovernmentHistory>();

            await _context.GovernmentHistories.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<GovernmentHistoryResponse>.Ok(
                entity.Adapt<GovernmentHistoryResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _context.GovernmentHistories.FindAsync(new object?[] { (object)countryCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "GovernmentHistory not found.",
                    HttpStatusCode.NotFound);
            }

            _context.GovernmentHistories.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<GovernmentHistoryResponse>> GetByKeyAsync(string countryCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<GovernmentHistory>(
                GovernmentHistoryQueries.GetByKey,
                new { CountryCode = countryCode, SequenceNumber = sequenceNumber });

            if (entity is null)
            {
                return Result<GovernmentHistoryResponse>.Fail(
                    "GovernmentHistory not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<GovernmentHistoryResponse>.Ok(entity.Adapt<GovernmentHistoryResponse>());
        }

        public async Task<Result<PaginatedResult<GovernmentHistoryResponse>>> GetAllAsync(GetGovernmentHistoriesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(GovernmentHistoryQueries.Count);

            var items = await _sql.QueryAsync<GovernmentHistory>(
                GovernmentHistoryQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<GovernmentHistoryResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<GovernmentHistoryResponse>>());

            return Result<PaginatedResult<GovernmentHistoryResponse>>.Ok(response);
        }

        public async Task<Result<GovernmentHistoryResponse>> UpdateAsync(string countryCode, int sequenceNumber, UpdateGovernmentHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.GovernmentHistories.FindAsync(new object?[] { (object)countryCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<GovernmentHistoryResponse>.Fail(
                    "GovernmentHistory not found.",
                    HttpStatusCode.NotFound);
            }

            entity.GovernmentTypeCode = request.GovernmentTypeCode;
            entity.From = request.From;
            entity.To = request.To;

            _context.GovernmentHistories.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<GovernmentHistoryResponse>.Ok(entity.Adapt<GovernmentHistoryResponse>());
        }
    }
}
