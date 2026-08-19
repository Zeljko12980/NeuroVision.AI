using BuildingBlocks.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationService.Application.Common.Request
{
    public record GetCountriesRequest(string? Search, string? GovernmentTypeCode, bool IncludeCapital) : PaginationRequest;

}
