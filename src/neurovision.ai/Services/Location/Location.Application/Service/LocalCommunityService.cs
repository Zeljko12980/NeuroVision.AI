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
    public class LocalCommunityService : ILocalCommunityService
    {
        private readonly ILocationDbContext _context;
        private readonly ISqlQueryExecutor _sql;
        private readonly ILogger<LocalCommunityService> _logger;

        public LocalCommunityService(
            ILocationDbContext context,
            ISqlQueryExecutor sql,
            ILogger<LocalCommunityService> logger)
        {
            _context = context;
            _sql = sql;
            _logger = logger;
        }

        public async Task<Result<LocalCommunityResponse>> AddAsync(CreateLocalCommunityRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                LocalCommunityQueries.Exists,
                new { CountryCode = request.CountryCode, MunicipalityCode = request.MunicipalityCode, Identifier = request.Identifier });

            if (exists > 0)
            {
                return Result<LocalCommunityResponse>.Fail(
                    "LocalCommunity already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<LocalCommunity>();

            await _context.LocalCommunities.AddAsync(entity, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<LocalCommunityResponse>.Ok(
                entity.Adapt<LocalCommunityResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string countryCode, int municipalityCode, int identifier, CancellationToken cancellationToken = default)
        {
            var entity = await _context.LocalCommunities.FindAsync(new object?[] { (object)countryCode, (object)municipalityCode, (object)identifier }, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "LocalCommunity not found.",
                    HttpStatusCode.NotFound);
            }

            _context.LocalCommunities.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<LocalCommunityResponse>> GetByKeyAsync(string countryCode, int municipalityCode, int identifier, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<LocalCommunity>(
                LocalCommunityQueries.GetByKey,
                new { CountryCode = countryCode, MunicipalityCode = municipalityCode, Identifier = identifier });

            if (entity is null)
            {
                return Result<LocalCommunityResponse>.Fail(
                    "LocalCommunity not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<LocalCommunityResponse>.Ok(entity.Adapt<LocalCommunityResponse>());
        }

        public async Task<Result<PaginatedResult<LocalCommunityResponse>>> GetAllAsync(GetLocalCommunitiesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(LocalCommunityQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<LocalCommunity>(
                LocalCommunityQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<LocalCommunityResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<LocalCommunityResponse>>());

            return Result<PaginatedResult<LocalCommunityResponse>>.Ok(response);
        }

        public async Task<Result<LocalCommunityResponse>> UpdateAsync(string countryCode, int municipalityCode, int identifier, UpdateLocalCommunityRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _context.LocalCommunities.FindAsync(new object?[] { (object)countryCode, (object)municipalityCode, (object)identifier }, cancellationToken);

            if (entity is null)
            {
                return Result<LocalCommunityResponse>.Fail(
                    "LocalCommunity not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.OfficeSettlementCode = request.OfficeSettlementCode;

            _context.LocalCommunities.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<LocalCommunityResponse>.Ok(entity.Adapt<LocalCommunityResponse>());
        }
    }
}
