using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Models.Commands;

public record RegisterAiModelVersionCommand(
    AiTaskType TaskType,
    string VersionLabel,
    string RunId,
    string WeightsPath,
    Guid RegisteredByUserId,
    bool SetActive = true) : IRequest<AiModelVersionResponse>;

public class RegisterAiModelVersionCommandHandler : IRequestHandler<RegisterAiModelVersionCommand, AiModelVersionResponse>
{
    private readonly IAiModelVersionRepository _models;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAiModelVersionCommandHandler(IAiModelVersionRepository models, IUnitOfWork unitOfWork)
    {
        _models = models;
        _unitOfWork = unitOfWork;
    }

    public async Task<AiModelVersionResponse> Handle(
        RegisterAiModelVersionCommand request,
        CancellationToken cancellationToken)
    {
        var existing = await _models.GetByRunIdAsync(request.RunId, cancellationToken);
        if (existing is not null && existing.TaskType != request.TaskType)
        {
            throw new BadRequestException(
                $"Run '{request.RunId}' is already registered for {existing.TaskType}.");
        }

        if (request.SetActive)
            await _models.DeactivateAllAsync(request.TaskType, cancellationToken);

        if (existing is not null)
        {
            existing.ReplaceRegistration(
                request.VersionLabel,
                request.WeightsPath,
                request.RegisteredByUserId,
                request.SetActive);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Map(existing);
        }

        var version = AiModelVersion.Create(
            request.TaskType,
            request.VersionLabel,
            request.RunId,
            request.WeightsPath,
            request.RegisteredByUserId,
            request.SetActive);

        await _models.AddAsync(version, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(version);
    }

    internal static AiModelVersionResponse Map(AiModelVersion v) => new(
        v.Id,
        v.TaskType.ToString(),
        v.VersionLabel,
        v.RunId,
        v.IsActive,
        v.RegisteredAt);
}

public record ActivateAiModelVersionCommand(Guid Id) : IRequest<AiModelVersionResponse>;

public class ActivateAiModelVersionCommandHandler : IRequestHandler<ActivateAiModelVersionCommand, AiModelVersionResponse>
{
    private readonly IAiModelVersionRepository _models;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateAiModelVersionCommandHandler(IAiModelVersionRepository models, IUnitOfWork unitOfWork)
    {
        _models = models;
        _unitOfWork = unitOfWork;
    }

    public async Task<AiModelVersionResponse> Handle(
        ActivateAiModelVersionCommand request,
        CancellationToken cancellationToken)
    {
        var version = await _models.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"AI model version {request.Id} was not found.");

        if (!version.IsActive)
        {
            await _models.DeactivateAllAsync(version.TaskType, cancellationToken);
            version.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return RegisterAiModelVersionCommandHandler.Map(version);
    }
}

public record UploadAiModelVersionCommand(
    string TaskTypeCode,
    string VersionLabel,
    string? RunId,
    Stream Content,
    string FileName,
    Guid RegisteredByUserId,
    bool SetActive = true) : IRequest<AiModelVersionResponse>;

public class UploadAiModelVersionCommandHandler : IRequestHandler<UploadAiModelVersionCommand, AiModelVersionResponse>
{
    private readonly IModelStorageService _storage;
    private readonly IAiModelTypeRepository _types;
    private readonly ISender _sender;

    public UploadAiModelVersionCommandHandler(
        IModelStorageService storage,
        IAiModelTypeRepository types,
        ISender sender)
    {
        _storage = storage;
        _types = types;
        _sender = sender;
    }

    public async Task<AiModelVersionResponse> Handle(
        UploadAiModelVersionCommand request,
        CancellationToken cancellationToken)
    {
        var modelType = await _types.GetByCodeAsync(request.TaskTypeCode.Trim(), cancellationToken)
            ?? throw new BadRequestException($"Unknown model type '{request.TaskTypeCode}'.");

        if (!Enum.TryParse<AiTaskType>(modelType.Code, true, out var taskType) || !Enum.IsDefined(taskType))
            throw new BadRequestException(
                $"Model type '{modelType.Code}' is not supported by the analysis pipeline.");

        var extension = Path.GetExtension(request.FileName);
        if (!string.Equals(extension, ".pt", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".pth", StringComparison.OrdinalIgnoreCase))
            throw new BadRequestException("Only .pt / .pth weight files are supported.");

        var runId = string.IsNullOrWhiteSpace(request.RunId)
            ? $"upload_{DateTime.UtcNow:yyyyMMddHHmmssfff}"
            : SanitizeRunId(request.RunId);

        var versionLabel = string.IsNullOrWhiteSpace(request.VersionLabel)
            ? runId
            : request.VersionLabel.Trim();

        var weightsPath = await _storage.SaveWeightsAsync(
            taskType,
            runId,
            request.Content,
            cancellationToken);

        return await _sender.Send(
            new RegisterAiModelVersionCommand(
                taskType,
                versionLabel,
                runId,
                weightsPath,
                request.RegisteredByUserId,
                request.SetActive),
            cancellationToken);
    }

    private static string SanitizeRunId(string runId)
    {
        var trimmed = runId.Trim();
        var chars = trimmed
            .Select(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_' ? ch : '_')
            .ToArray();
        var sanitized = new string(chars);
        return string.IsNullOrWhiteSpace(sanitized) ? $"upload_{Guid.NewGuid():N}" : sanitized;
    }
}
