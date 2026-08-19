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
    public class LegalSuccessorService : ILegalSuccessorService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<LegalSuccessorService> _logger;

        public LegalSuccessorService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<LegalSuccessorService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<LegalSuccessorResponse>> AddAsync(CreateLegalSuccessorRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                LegalSuccessorQueries.Exists,
                new { SuccessorCountryCode = request.SuccessorCountryCode, PredecessorCountryCode = request.PredecessorCountryCode });

            if (exists > 0)
            {
                return Result<LegalSuccessorResponse>.Fail(
                    "LegalSuccessor already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<LegalSuccessor>();

            await _context.LegalSuccessors.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<LegalSuccessorResponse>.Ok(
                entity.Adapt<LegalSuccessorResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string successorCountryCode, string predecessorCountryCode, CancellationToken cancellationToken = default)
        {
            var entity = await _context.LegalSuccessors.FindAsync(new object?[] { (object)successorCountryCode, (object)predecessorCountryCode }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "LegalSuccessor not found.",
                    HttpStatusCode.NotFound);
            }

            _context.LegalSuccessors.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<LegalSuccessorResponse>> GetByKeyAsync(string successorCountryCode, string predecessorCountryCode, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<LegalSuccessor>(
                LegalSuccessorQueries.GetByKey,
                new { SuccessorCountryCode = successorCountryCode, PredecessorCountryCode = predecessorCountryCode });

            if (entity is null)
            {
                return Result<LegalSuccessorResponse>.Fail(
                    "LegalSuccessor not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<LegalSuccessorResponse>.Ok(entity.Adapt<LegalSuccessorResponse>());
        }

        public async Task<Result<PaginatedResult<LegalSuccessorResponse>>> GetAllAsync(GetLegalSuccessorsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(LegalSuccessorQueries.Count);

            var items = await _sql.QueryAsync<LegalSuccessor>(
                LegalSuccessorQueries.GetPaged,
                new
                {
                    
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<LegalSuccessorResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<LegalSuccessorResponse>>());

            return Result<PaginatedResult<LegalSuccessorResponse>>.Ok(response);
        }
    }
}
