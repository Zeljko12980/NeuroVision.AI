using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.ClinicalCatalogs;

public record ClinicalCatalogItemResponse(string Code, string Name, string? Description);

public record ClinicalCatalogsBundleResponse(
    IReadOnlyList<ClinicalCatalogItemResponse> Grades,
    IReadOnlyList<ClinicalCatalogItemResponse> OperabilityStatuses,
    IReadOnlyList<ClinicalCatalogItemResponse> SpreadStatuses,
    IReadOnlyList<ClinicalCatalogItemResponse> TreatmentOptions);

public record GetClinicalCatalogsBundleQuery : IRequest<ClinicalCatalogsBundleResponse>;

public class GetClinicalCatalogsBundleQueryHandler
    : IRequestHandler<GetClinicalCatalogsBundleQuery, ClinicalCatalogsBundleResponse>
{
    private readonly IClinicalCatalogRepository _catalogs;

    public GetClinicalCatalogsBundleQueryHandler(IClinicalCatalogRepository catalogs) =>
        _catalogs = catalogs;

    public async Task<ClinicalCatalogsBundleResponse> Handle(
        GetClinicalCatalogsBundleQuery request,
        CancellationToken cancellationToken)
    {
        var grades = await _catalogs.GetByCategoryAsync(ClinicalCatalogCategory.Grade, cancellationToken);
        var operability = await _catalogs.GetByCategoryAsync(ClinicalCatalogCategory.Operability, cancellationToken);
        var spread = await _catalogs.GetByCategoryAsync(ClinicalCatalogCategory.Spread, cancellationToken);
        var treatments = await _catalogs.GetByCategoryAsync(ClinicalCatalogCategory.TreatmentOption, cancellationToken);

        return new ClinicalCatalogsBundleResponse(
            grades.Select(Map).ToList(),
            operability.Select(Map).ToList(),
            spread.Select(Map).ToList(),
            treatments.Select(Map).ToList());
    }

    internal static ClinicalCatalogItemResponse Map(ClinicalCatalogItem item) =>
        new(item.Code, item.Name, item.Description);
}

public record GetClinicalCatalogByCategoryQuery(
    ClinicalCatalogCategory Category,
    int PageIndex = 0,
    int PageSize = 100,
    string? Search = null) : IRequest<PaginatedResult<ClinicalCatalogItemResponse>>;

public class GetClinicalCatalogByCategoryQueryHandler
    : IRequestHandler<GetClinicalCatalogByCategoryQuery, PaginatedResult<ClinicalCatalogItemResponse>>
{
    private readonly IClinicalCatalogRepository _catalogs;

    public GetClinicalCatalogByCategoryQueryHandler(IClinicalCatalogRepository catalogs) =>
        _catalogs = catalogs;

    public async Task<PaginatedResult<ClinicalCatalogItemResponse>> Handle(
        GetClinicalCatalogByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageIndex = Math.Max(request.PageIndex, 0);
        var (items, total) = await _catalogs.SearchByCategoryAsync(
            request.Category,
            request.Search,
            pageIndex,
            pageSize,
            cancellationToken);

        return new PaginatedResult<ClinicalCatalogItemResponse>(
            pageIndex,
            pageSize,
            total,
            items.Select(GetClinicalCatalogsBundleQueryHandler.Map).ToList());
    }
}

public record CreateClinicalCatalogItemCommand(
    ClinicalCatalogCategory Category,
    string Code,
    string Name,
    string? Description) : IRequest<ClinicalCatalogItemResponse>;

public class CreateClinicalCatalogItemCommandHandler
    : IRequestHandler<CreateClinicalCatalogItemCommand, ClinicalCatalogItemResponse>
{
    private readonly IClinicalCatalogRepository _catalogs;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClinicalCatalogItemCommandHandler(
        IClinicalCatalogRepository catalogs,
        IUnitOfWork unitOfWork)
    {
        _catalogs = catalogs;
        _unitOfWork = unitOfWork;
    }

    public async Task<ClinicalCatalogItemResponse> Handle(
        CreateClinicalCatalogItemCommand request,
        CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();
        if (await _catalogs.GetAsync(request.Category, code, cancellationToken) is not null)
            throw new BadRequestException($"Catalog item '{code}' already exists for this category.");

        var entity = ClinicalCatalogItem.Create(request.Category, code, request.Name, request.Description);
        await _catalogs.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return GetClinicalCatalogsBundleQueryHandler.Map(entity);
    }
}
