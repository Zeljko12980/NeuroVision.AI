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
    public class CountryCompositionService : ICountryCompositionService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<CountryCompositionService> _logger;

        public CountryCompositionService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<CountryCompositionService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<CountryCompositionResponse>> AddAsync(CreateCountryCompositionRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                CountryCompositionQueries.Exists,
                new { UnionCountryCode = request.UnionCountryCode, MemberCountryCode = request.MemberCountryCode, SequenceNumber = request.SequenceNumber });

            if (exists > 0)
            {
                return Result<CountryCompositionResponse>.Fail(
                    "CountryComposition already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<CountryComposition>();

            await _context.CountryCompositions.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CountryCompositionResponse>.Ok(
                entity.Adapt<CountryCompositionResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string unionCountryCode, string memberCountryCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _context.CountryCompositions.FindAsync(new object?[] { (object)unionCountryCode, (object)memberCountryCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "CountryComposition not found.",
                    HttpStatusCode.NotFound);
            }

            _context.CountryCompositions.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<CountryCompositionResponse>> GetByKeyAsync(string unionCountryCode, string memberCountryCode, int sequenceNumber, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<CountryComposition>(
                CountryCompositionQueries.GetByKey,
                new { UnionCountryCode = unionCountryCode, MemberCountryCode = memberCountryCode, SequenceNumber = sequenceNumber });

            if (entity is null)
            {
                return Result<CountryCompositionResponse>.Fail(
                    "CountryComposition not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<CountryCompositionResponse>.Ok(entity.Adapt<CountryCompositionResponse>());
        }

        public async Task<Result<PaginatedResult<CountryCompositionResponse>>> GetAllAsync(GetCountryCompositionsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(CountryCompositionQueries.Count);

            var items = await _sql.QueryAsync<CountryComposition>(
                CountryCompositionQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<CountryCompositionResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<CountryCompositionResponse>>());

            return Result<PaginatedResult<CountryCompositionResponse>>.Ok(response);
        }

        public async Task<Result<CountryCompositionResponse>> UpdateAsync(string unionCountryCode, string memberCountryCode, int sequenceNumber, UpdateCountryCompositionRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.CountryCompositions.FindAsync(new object?[] { (object)unionCountryCode, (object)memberCountryCode, (object)sequenceNumber }, cancellationToken);

            if (entity is null)
            {
                return Result<CountryCompositionResponse>.Fail(
                    "CountryComposition not found.",
                    HttpStatusCode.NotFound);
            }

            entity.From = request.From;
            entity.To = request.To;

            _context.CountryCompositions.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<CountryCompositionResponse>.Ok(entity.Adapt<CountryCompositionResponse>());
        }
    }
}
