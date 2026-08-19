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
    public class MunicipalityService : IMunicipalityService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<MunicipalityService> _logger;

        public MunicipalityService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<MunicipalityService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<MunicipalityResponse>> AddAsync(CreateMunicipalityRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                MunicipalityQueries.Exists,
                new { CountryCode = request.CountryCode, Code = request.Code });

            if (exists > 0)
            {
                return Result<MunicipalityResponse>.Fail(
                    "Municipality already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<Municipality>();

            await _context.Municipalities.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<MunicipalityResponse>.Ok(
                entity.Adapt<MunicipalityResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int code, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Municipalities.FindAsync(new object?[] { (object)countryCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "Municipality not found.",
                    HttpStatusCode.NotFound);
            }

            _context.Municipalities.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<MunicipalityResponse>> GetByKeyAsync(string countryCode, int code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<Municipality>(
                MunicipalityQueries.GetByKey,
                new { CountryCode = countryCode, Code = code });

            if (entity is null)
            {
                return Result<MunicipalityResponse>.Fail(
                    "Municipality not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<MunicipalityResponse>.Ok(entity.Adapt<MunicipalityResponse>());
        }

        public async Task<Result<PaginatedResult<MunicipalityResponse>>> GetAllAsync(GetMunicipalitiesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(MunicipalityQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<Municipality>(
                MunicipalityQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<MunicipalityResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<MunicipalityResponse>>());

            return Result<PaginatedResult<MunicipalityResponse>>.Ok(response);
        }

        public async Task<Result<MunicipalityResponse>> UpdateAsync(string countryCode, int code, UpdateMunicipalityRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Municipalities.FindAsync(new object?[] { (object)countryCode, (object)code }, cancellationToken);

            if (entity is null)
            {
                return Result<MunicipalityResponse>.Fail(
                    "Municipality not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.SeatSettlementCode = request.SeatSettlementCode;

            _context.Municipalities.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<MunicipalityResponse>.Ok(entity.Adapt<MunicipalityResponse>());
        }
    }
}
