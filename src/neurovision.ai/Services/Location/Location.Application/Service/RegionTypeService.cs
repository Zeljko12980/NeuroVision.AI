using BuildingBlocks.Dapper;
using BuildingBlocks.Pagination;
using BuildingBlocks.Persistence;
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
    public class RegionTypeService : IRegionTypeService
    {
        private readonly IRepository<RegionType, string> _repository;
        private readonly ISqlQueryExecutor _sql;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegionTypeService> _logger;

        public RegionTypeService(
            IRepository<RegionType, string> repository,
            ISqlQueryExecutor sql,
            IUnitOfWork unitOfWork,
            ILogger<RegionTypeService> logger)
        {
            _repository = repository;
            _sql = sql;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<RegionTypeResponse>> AddAsync(CreateRegionTypeRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                RegionTypeQueries.Exists,
                new { code = request.Code });

            if (exists > 0)
            {
                return Result<RegionTypeResponse>.Fail(
                    "RegionType already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<RegionType>();

            await _repository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<RegionTypeResponse>.Ok(
                entity.Adapt<RegionTypeResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "RegionType not found.",
                    HttpStatusCode.NotFound);
            }

            _repository.Delete(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<RegionTypeResponse>> GetByKeyAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<RegionType>(
                RegionTypeQueries.GetByKey,
                new { code = code });

            if (entity is null)
            {
                return Result<RegionTypeResponse>.Fail(
                    "RegionType not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<RegionTypeResponse>.Ok(entity.Adapt<RegionTypeResponse>());
        }

        public async Task<Result<PaginatedResult<RegionTypeResponse>>> GetAllAsync(GetRegionTypesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(RegionTypeQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<RegionType>(
                RegionTypeQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<RegionTypeResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<RegionTypeResponse>>());

            return Result<PaginatedResult<RegionTypeResponse>>.Ok(response);
        }

        public async Task<Result<RegionTypeResponse>> UpdateAsync(string code, UpdateRegionTypeRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<RegionTypeResponse>.Fail(
                    "RegionType not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<RegionTypeResponse>.Ok(entity.Adapt<RegionTypeResponse>());
        }
    }
}
