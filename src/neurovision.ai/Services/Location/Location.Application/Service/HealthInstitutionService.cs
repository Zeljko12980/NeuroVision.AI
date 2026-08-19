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
    public class HealthInstitutionService : IHealthInstitutionService
    {
        private readonly IRepository<HealthInstitution, int> _repository;
        private readonly ISqlQueryExecutor _sql;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HealthInstitutionService> _logger;

        public HealthInstitutionService(
            IRepository<HealthInstitution, int> repository,
            ISqlQueryExecutor sql,
            IUnitOfWork unitOfWork,
            ILogger<HealthInstitutionService> logger)
        {
            _repository = repository;
            _sql = sql;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<HealthInstitutionResponse>> AddAsync(CreateHealthInstitutionRequest request, CancellationToken cancellationToken = default)
        {
            var entity = request.Adapt<HealthInstitution>();

            await _repository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<HealthInstitutionResponse>.Ok(
                entity.Adapt<HealthInstitutionResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                return Result<bool>.Fail(
                    "HealthInstitution not found.",
                    HttpStatusCode.NotFound);
            }

            _repository.Delete(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Ok(true, HttpStatusCode.OK);
        }

        public async Task<Result<HealthInstitutionResponse>> GetByKeyAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _sql.QuerySingleAsync<HealthInstitution>(
                HealthInstitutionQueries.GetByKey,
                new { id = id });

            if (entity is null)
            {
                return Result<HealthInstitutionResponse>.Fail(
                    "HealthInstitution not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<HealthInstitutionResponse>.Ok(entity.Adapt<HealthInstitutionResponse>());
        }

        public async Task<Result<PaginatedResult<HealthInstitutionResponse>>> GetAllAsync(GetHealthInstitutionsRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(HealthInstitutionQueries.Count, new { request.Search });

            var items = await _sql.QueryAsync<HealthInstitution>(
                HealthInstitutionQueries.GetPaged,
                new
                {
                    request.Search,
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<HealthInstitutionResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                items.Adapt<List<HealthInstitutionResponse>>());

            return Result<PaginatedResult<HealthInstitutionResponse>>.Ok(response);
        }

        public async Task<Result<HealthInstitutionResponse>> UpdateAsync(int id, UpdateHealthInstitutionRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);

            if (entity is null)
            {
                return Result<HealthInstitutionResponse>.Fail(
                    "HealthInstitution not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.TypeCode = request.TypeCode;
            entity.CountryCode = request.CountryCode;
            entity.SettlementCode = request.SettlementCode;
            entity.Address = request.Address;
            entity.BedCount = request.BedCount;
            entity.FoundingDate = request.FoundingDate;
            entity.Phone = request.Phone;

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<HealthInstitutionResponse>.Ok(entity.Adapt<HealthInstitutionResponse>());
        }
    }
}
