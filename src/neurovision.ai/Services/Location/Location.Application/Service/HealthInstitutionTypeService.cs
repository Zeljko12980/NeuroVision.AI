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
    public class HealthInstitutionTypeService : IHealthInstitutionTypeService
    {
        private readonly IRepository<HealthInstitutionType, string> _repository;
        private readonly ISqlQueryExecutor _sql;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HealthInstitutionTypeService> _logger;

        public HealthInstitutionTypeService(
            IRepository<HealthInstitutionType, string> repository,
            ISqlQueryExecutor sql,
            IUnitOfWork unitOfWork,
            ILogger<HealthInstitutionTypeService> logger)
        {
            _repository = repository;
            _sql = sql;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<HealthInstitutionTypeResponse>> AddAsync(CreateHealthInstitutionTypeRequest request, CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                HealthInstitutionTypeQueries.Exists,
                new { code = request.Code });

            if (exists > 0)
            {
                return Result<HealthInstitutionTypeResponse>.Fail(
                    "HealthInstitutionType already exists.",
                    HttpStatusCode.Conflict);
            }

            var entity = request.Adapt<HealthInstitutionType>();

            await _repository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<HealthInstitutionTypeResponse>.Ok(
                entity.Adapt<HealthInstitutionTypeResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "HealthInstitutionType not found.",
                    HttpStatusCode.NotFound);
            }

            _repository.Delete(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<HealthInstitutionTypeResponse>> GetByKeyAsync(string code, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<HealthInstitutionType>(
                HealthInstitutionTypeQueries.GetByKey,
                new { code = code });

            if (entity is null)
            {
                return Result<HealthInstitutionTypeResponse>.Fail(
                    "HealthInstitutionType not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<HealthInstitutionTypeResponse>.Ok(entity.Adapt<HealthInstitutionTypeResponse>());
        }

        public async Task<Result<PaginatedResult<HealthInstitutionTypeResponse>>> GetAllAsync(GetHealthInstitutionTypesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(HealthInstitutionTypeQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<HealthInstitutionType>(
                HealthInstitutionTypeQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<HealthInstitutionTypeResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<HealthInstitutionTypeResponse>>());

            return Result<PaginatedResult<HealthInstitutionTypeResponse>>.Ok(response);
        }

        public async Task<Result<HealthInstitutionTypeResponse>> UpdateAsync(string code, UpdateHealthInstitutionTypeRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(code, cancellationToken);

            if (entity is null)
            {
                return Result<HealthInstitutionTypeResponse>.Fail(
                    "HealthInstitutionType not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.Description = request.Description;

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<HealthInstitutionTypeResponse>.Ok(entity.Adapt<HealthInstitutionTypeResponse>());
        }
    }
}
