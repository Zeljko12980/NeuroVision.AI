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
    public class GovernmentTypeService : IGovernmentTypeService
    {
        private readonly IRepository<GovernmentType, string> _repository;
        private readonly ISqlQueryExecutor _sql;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GovernmentTypeService> _logger;

        public GovernmentTypeService(
            IRepository<GovernmentType, string> repository,
            ISqlQueryExecutor sql,
            IUnitOfWork unitOfWork,
            ILogger<GovernmentTypeService> logger)
        {
            _repository = repository;
            _sql = sql;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<GovernmentTypeResponse>> AddAsync(CreateGovernmentTypeRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                GovernmentTypeQueries.Exists,
                new { code = request.Code });

            if (exists > 0)
            {
                return Result<GovernmentTypeResponse>.Fail(
                    "GovernmentType already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<GovernmentType>();

            await _repository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<GovernmentTypeResponse>.Ok(
                entity.Adapt<GovernmentTypeResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "GovernmentType not found.",
                    HttpStatusCode.NotFound);
            }

            _repository.Delete(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<GovernmentTypeResponse>> GetByKeyAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<GovernmentType>(
                GovernmentTypeQueries.GetByKey,
                new { code = code });

            if (entity is null)
            {
                return Result<GovernmentTypeResponse>.Fail(
                    "GovernmentType not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<GovernmentTypeResponse>.Ok(entity.Adapt<GovernmentTypeResponse>());
        }

        public async Task<Result<PaginatedResult<GovernmentTypeResponse>>> GetAllAsync(GetGovernmentTypesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(GovernmentTypeQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<GovernmentType>(
                GovernmentTypeQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<GovernmentTypeResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<GovernmentTypeResponse>>());

            return Result<PaginatedResult<GovernmentTypeResponse>>.Ok(response);
        }

        public async Task<Result<GovernmentTypeResponse>> UpdateAsync(string code, UpdateGovernmentTypeRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<GovernmentTypeResponse>.Fail(
                    "GovernmentType not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<GovernmentTypeResponse>.Ok(entity.Adapt<GovernmentTypeResponse>());
        }
    }
}
