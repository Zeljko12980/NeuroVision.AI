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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace LocationService.Application.Service
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country, string> _repository;
        private readonly ISqlQueryExecutor _sql;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            IRepository<Country, string> repository,
            ISqlQueryExecutor sql,
            IUnitOfWork unitOfWork,
            ILogger<CountryService> logger)
        {
            _repository = repository;
            _sql = sql;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<CountryResponse>> AddAsync(
      CreateCountryRequest request,
      CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                CountryQueries.Exists,
                new { request.Code });


            if (exists > 0)
            {
                return Result<CountryResponse>.Fail(
                    "Country already exists.",
                    HttpStatusCode.Conflict);
            }


            var entity = new Country
            {
                Code = request.Code,

                Name = request.Name,

                FoundingDate = request.FoundingDate,

                CapitalSettlementCode =
                    request.CapitalSettlementCode,

                GovernmentTypeCode =
                    request.GovernmentTypeCode,

                CallingCode =
                    request.CallingCode,


                Flag =
                    await ConvertFileToBytesAsync(
                        request.Flag,
                        cancellationToken),


                CoatOfArms =
                    await ConvertFileToBytesAsync(
                        request.CoatOfArms,
                        cancellationToken),


                Anthem =
                    await ConvertFileToBytesAsync(
                        request.Anthem,
                        cancellationToken)
            };


            await _repository.AddAsync(
                entity,
                cancellationToken);


            await _unitOfWork.SaveChangesAsync(
                cancellationToken);



            return Result<CountryResponse>.Ok(
                entity.Adapt<CountryResponse>(),
                HttpStatusCode.Created);
        }

        public async Task<Result<bool>> DeleteAsync(
     string code,
     CancellationToken cancellationToken = default)
        {
            var exists = await _sql.QuerySingleAsync<int>(
                CountryQueries.Exists,
                new { Code = code });

            if (exists == 0)
            {
                return Result<bool>.Fail(
                    "Country not found.",
                    HttpStatusCode.NotFound);
            }

            var country = await _repository.GetByIdAsync(
                code,
                cancellationToken);

            if (country is null)
            {
                return Result<bool>.Fail(
                    "Country not found.",
                    HttpStatusCode.NotFound);
            }

            _repository.Delete(country);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result<bool>.Ok(
                true,
                HttpStatusCode.OK);
        }

        public async Task<Result<PaginatedResult<CountryResponse>>> GetAllAsync(GetCountriesRequest request, CancellationToken cancellationToken = default)
        {
            var total = await _sql.QuerySingleAsync<int>(
           CountryQueries.Count);

            var countries = await _sql.QueryAsync<Country>(
                CountryQueries.GetPaged,
                new
                {
                    request.PageSize,
                    Offset = request.PageIndex * request.PageSize
                });

            var response = new PaginatedResult<CountryResponse>(
                request.PageIndex,
                request.PageSize,
                total,
                countries.Adapt<List<CountryResponse>>());

            return Result<PaginatedResult<CountryResponse>>.Ok(response);
        }

        public async Task<Result<CountryResponse>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var country = await _sql.QuerySingleAsync<Country>(
            CountryQueries.GetByCode,
            new { Code = code });

            if (country is null)
            {
                return Result<CountryResponse>.Fail(
                    "Country not found.",
                    HttpStatusCode.NotFound);
            }

            return Result<CountryResponse>.Ok(
                country.Adapt<CountryResponse>());
        }

        public async Task<Result<CountryResponse>> UpdateAsync(string code, UpdateCountryRequest request, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(
             code,
             cancellationToken);

            if (entity is null)
            {
                return Result<CountryResponse>.Fail(
                    "Country not found.",
                    HttpStatusCode.NotFound);
            }

            entity.Name = request.Name;
            entity.FoundingDate = request.FoundingDate;
            entity.GovernmentTypeCode = request.GovernmentTypeCode;
            entity.CapitalSettlementCode = request.CapitalSettlementCode;
            entity.CallingCode = request.CallingCode;
            if (request.Anthem != null)
            {
                entity.Anthem =
                    await ConvertFileToBytesAsync(
                        request.Anthem,
                        cancellationToken);
            }


            if (request.Flag != null)
            {
                entity.Flag =
                    await ConvertFileToBytesAsync(
                        request.Flag,
                        cancellationToken);
            }


            if (request.CoatOfArms != null)
            {
                entity.CoatOfArms =
                    await ConvertFileToBytesAsync(
                        request.CoatOfArms,
                        cancellationToken);
            }

            _repository.Update(entity);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return Result<CountryResponse>.Ok(
                entity.Adapt<CountryResponse>());
        }
    private async Task<byte[]?> ConvertFileToBytesAsync(
    IFormFile? file,
    CancellationToken cancellationToken = default)
        {
            if (file == null)
                return null;

            await using var memoryStream = new MemoryStream();

            await file.CopyToAsync(
                memoryStream,
                cancellationToken);

            return memoryStream.ToArray();
        }
    }
}
