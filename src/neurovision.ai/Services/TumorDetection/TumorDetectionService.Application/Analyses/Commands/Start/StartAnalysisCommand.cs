using BuildingBlocks.Exceptions;
using BuildingBlocks.Persistence;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Application.Common.Responses;
using TumorDetectionService.Domain.Entities;
using TumorDetectionService.Domain.Enums;

namespace TumorDetectionService.Application.Analyses.Commands.Start;

public record StartAnalysisCommand(Guid BrainScanId, TumorActor Actor) : IRequest<AnalysisResponse>;

public class StartAnalysisCommandHandler : IRequestHandler<StartAnalysisCommand, AnalysisResponse>
{
    private readonly IBrainScanRepository _scans;
    private readonly ITumorAnalysisRepository _analyses;
    private readonly IAiModelVersionRepository _models;
    private readonly IAnalysisNotificationPublisher _publisher;
    private readonly IAnalysisJobRunner _jobRunner;
    private readonly IUnitOfWork _unitOfWork;

    public StartAnalysisCommandHandler(
        IBrainScanRepository scans,
        ITumorAnalysisRepository analyses,
        IAiModelVersionRepository models,
        IAnalysisNotificationPublisher publisher,
        IAnalysisJobRunner jobRunner,
        IUnitOfWork unitOfWork)
    {
        _scans = scans;
        _analyses = analyses;
        _models = models;
        _publisher = publisher;
        _jobRunner = jobRunner;
        _unitOfWork = unitOfWork;
    }

    public async Task<AnalysisResponse> Handle(StartAnalysisCommand request, CancellationToken cancellationToken)
    {
        var scan = await _scans.GetByIdForProcessingAsync(request.BrainScanId, cancellationToken)
            ?? throw new NotFoundException($"Brain scan {request.BrainScanId} not found.");
        TumorAccess.EnsureCanAccessScan(request.Actor, scan);

        var analysis = TumorAnalysis.Create(scan.Id, request.Actor.UserId);
        analysis.MarkProcessing();

        var detRun = (await _models.GetActiveAsync(AiTaskType.Detection, cancellationToken))?.RunId;
        var clsRun = (await _models.GetActiveAsync(AiTaskType.Classification, cancellationToken))?.RunId;
        var segRun = (await _models.GetActiveAsync(AiTaskType.Segmentation, cancellationToken))?.RunId;
        analysis.SetModelRuns(detRun, clsRun, segRun);

        await _analyses.AddAsync(analysis, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _publisher.PublishAsync(
            new AnalysisStatusNotification(
                analysis.Id,
                scan.Id,
                scan.PatientId,
                analysis.Status.ToString()),
            cancellationToken);

        _jobRunner.StartAnalysisJob(
            analysis.Id,
            scan.StoredFilePath,
            detRun,
            clsRun,
            segRun);

        var loaded = await _analyses.GetByIdWithDetailsAsync(analysis.Id, cancellationToken);
        return AnalysisMapper.ToResponse(loaded!);
    }
}
