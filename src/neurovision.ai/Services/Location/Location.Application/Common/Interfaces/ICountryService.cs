using BuildingBlocks.Pagination;
using BuildingBlocks.Results;
using LocationService.Application.Common.Request;
using LocationService.Application.Common.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationService.Application.Common.Interfaces
{
    public interface ICountryService
    {
        Task<Result<CountryResponse>> AddAsync(
            CreateCountryRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<CountryResponse>> UpdateAsync(
            string code,
            UpdateCountryRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<bool>> DeleteAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<CountryResponse>> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Result<PaginatedResult<CountryResponse>>> GetAllAsync(
            GetCountriesRequest request,
            CancellationToken cancellationToken = default);
    }
}
