using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Corrections.Commands;

public record ApplyManualCorrectionCommand(
    Guid TumorAnalysisId,
    TumorActor Actor,
    TumorClassType CorrectedClass,
    string? Notes) : IRequest<AnalysisResponse>;

public class ApplyManualCorrectionCommandHandler : IRequestHandler<ApplyManualCorrectionCommand, AnalysisResponse>
{
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IUnitOfWork _unitOfWork;

    public ApplyManualCorrectionCommandHandler(ITumorAnalysisRepository analyses, IUnitOfWork unitOfWork)
    {
        _analyses = analyses;
        _unitOfWork = unitOfWork;
    }

    public async Task<AnalysisResponse> Handle(ApplyManualCorrectionCommand request, CancellationToken cancellationToken)
    {
        var analysis = await _analyses.GetByIdWithDetailsAsync(request.TumorAnalysisId, cancellationToken)
            ?? throw new NotFoundException($"Analysis {request.TumorAnalysisId} not found.");
        TumorAccess.EnsureCanAccessAnalysis(request.Actor, analysis);
        TumorAccess.EnsureStaff(request.Actor);

        if (analysis.Status is not (AnalysisStatus.Completed or AnalysisStatus.Corrected))
            throw new InvalidOperationException("Only completed analyses can be manually corrected.");

        if (analysis.Classification is not null)
            analysis.Classification.ApplyCorrection(request.CorrectedClass);
        else
            analysis.AttachClassification(ClassificationResult.Create(
                analysis.Id,
                request.CorrectedClass,
                1.0,
                "{}"));

        analysis.AttachManualCorrection(ManualCorrection.Create(
            analysis.Id,
            request.CorrectedClass,
            request.Actor.UserId,
            request.Notes));

        analysis.MarkCorrected();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await _analyses.GetByIdWithDetailsAsync(analysis.Id, cancellationToken);
        return AnalysisMapper.ToResponse(updated!);
    }
}
