using BuildingBlocks.Exceptions;
using MediatR;
using TumorDetectionService.Application.Common;
using TumorDetectionService.Application.Common.Interfaces;
using TumorDetectionService.Application.Common.Responses;

namespace TumorDetectionService.Application.Scans.Queries;

public record GetBrainScanImageQuery(Guid ScanId, TumorActor Actor) : IRequest<AnalysisFileResult>;

public class GetBrainScanImageQueryHandler : IRequestHandler<GetBrainScanImageQuery, AnalysisFileResult>
{
    private readonly IBrainScanRepository _scans;

    public GetBrainScanImageQueryHandler(IBrainScanRepository scans) => _scans = scans;

    public async Task<AnalysisFileResult> Handle(
        GetBrainScanImageQuery request,
        CancellationToken cancellationToken)
    {
        var scan = await _scans.GetByIdAsync(request.ScanId, cancellationToken)
            ?? throw new NotFoundException($"Brain scan {request.ScanId} not found.");

        TumorAccess.EnsureCanAccessScan(request.Actor, scan);

        if (string.IsNullOrWhiteSpace(scan.StoredFilePath) || !File.Exists(scan.StoredFilePath))
            throw new NotFoundException($"Image was not found for scan {request.ScanId}.");

        return new AnalysisFileResult(scan.StoredFilePath, scan.FileName);
    }
}
