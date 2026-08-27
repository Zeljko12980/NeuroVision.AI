using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.ModelTypes;

public record AiModelTypeResponse(string Code, string Name, string? Description);

public record GetAiModelTypesQuery(int PageIndex = 0, int PageSize = 100, string? Search = null)
    : IRequest<PaginatedResult<AiModelTypeResponse>>;

public class GetAiModelTypesQueryHandler
    : IRequestHandler<GetAiModelTypesQuery, PaginatedResult<AiModelTypeResponse>>
{
    private readonly IAiModelTypeRepository _types;

    public GetAiModelTypesQueryHandler(IAiModelTypeRepository types) => _types = types;

    public async Task<PaginatedResult<AiModelTypeResponse>> Handle(
        GetAiModelTypesQuery request,
        CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageIndex = Math.Max(request.PageIndex, 0);
        var (items, total) = await _types.SearchAsync(request.Search, pageIndex, pageSize, cancellationToken);

        return new PaginatedResult<AiModelTypeResponse>(
            pageIndex,
            pageSize,
            total,
            items.Select(Map).ToList());
    }

    internal static AiModelTypeResponse Map(AiModelType type) =>
        new(type.Code, type.Name, type.Description);
}

public record CreateAiModelTypeCommand(string Code, string Name, string? Description)
    : IRequest<AiModelTypeResponse>;

public class CreateAiModelTypeCommandHandler
    : IRequestHandler<CreateAiModelTypeCommand, AiModelTypeResponse>
{
    private readonly IAiModelTypeRepository _types;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAiModelTypeCommandHandler(IAiModelTypeRepository types, IUnitOfWork unitOfWork)
    {
        _types = types;
        _unitOfWork = unitOfWork;
    }

    public async Task<AiModelTypeResponse> Handle(
        CreateAiModelTypeCommand request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AiTaskType>(request.Code, true, out var taskType) || !Enum.IsDefined(taskType))
            throw new BadRequestException(
                "Code must be a supported pipeline task: Detection, Classification, or Segmentation.");

        var code = taskType.ToString();
        if (await _types.GetByCodeAsync(code, cancellationToken) is not null)
            throw new BadRequestException($"Model type '{code}' already exists.");

        var entity = AiModelType.Create(code, request.Name, request.Description);
        await _types.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return GetAiModelTypesQueryHandler.Map(entity);
    }
}
