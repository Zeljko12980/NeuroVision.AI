using System.Globalization;
using System.Net;
using System.Text;
using TumorDetectionService.Application.Common.Mapping;
using TumorDetectionService.Domain.Entities;

namespace TumorDetectionService.Application.Reports;

public static class AnalysisReportDataBuilder
{
    public static Dictionary<string, string> Build(TumorAnalysis analysis, string? doctorName = null)
    {
        var classification = analysis.Classification?.PredictedClass.ToString() ?? "—";
        var classificationConfidence = analysis.Classification?.Confidence;
        var tumorArea = analysis.Segmentation?.TumorAreaRatio;
        var correction = analysis.ManualCorrection;
        var physicianName = string.IsNullOrWhiteSpace(doctorName) ? "Attending Physician" : doctorName.Trim();

        var correctionSection = correction is null
            ? string.Empty
            : $"""
               <div class="section">
                   <h2>Manual correction</h2>
                   <table>
                       <tr><th>Corrected class</th><td>{WebUtility.HtmlEncode(correction.CorrectedClass.ToString())}</td></tr>
                       <tr><th>Corrected at</th><td>{correction.CorrectedAt:yyyy-MM-dd HH:mm} UTC</td></tr>
                       <tr><th>Notes</th><td>{WebUtility.HtmlEncode(correction.Notes ?? "—")}</td></tr>
                   </table>
               </div>
               """;

        var commentsSection = BuildCommentsSection(analysis.Comments);

        return new Dictionary<string, string>
        {
            ["@Model.ReportTitle"] = "Brain Tumor Analysis Report",
            ["@Model.AnalysisId"] = analysis.Id.ToString(),
            ["@Model.ScanFileName"] = WebUtility.HtmlEncode(analysis.BrainScan.FileName),
            ["@Model.ScanType"] = analysis.BrainScan.ScanType.ToString(),
            ["@Model.AnalysisStatus"] = analysis.Status.ToString(),
            ["@Model.RequestedAt"] = analysis.RequestedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            ["@Model.CompletedAt"] = analysis.CompletedAt?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "—",
            ["@Model.ClassificationClass"] = WebUtility.HtmlEncode(classification),
            ["@Model.ClassificationConfidence"] = classificationConfidence?.ToString("P1", CultureInfo.InvariantCulture) ?? "—",
            ["@Model.TumorAreaRatio"] = tumorArea?.ToString("P2", CultureInfo.InvariantCulture) ?? "—",
            ["@Model.OverallConfidence"] = analysis.OverallConfidence?.ToString("P1", CultureInfo.InvariantCulture) ?? "—",
            ["@Model.DetectionCount"] = analysis.Detections.Count.ToString(CultureInfo.InvariantCulture),
            ["@Model.DetectionsTable"] = BuildDetectionsTable(analysis.Detections),
            ["@Model.AnnotatedImage"] = BuildAnnotatedImageHtml(analysis),
            ["@Model.CorrectionSection"] = correctionSection,
            ["@Model.CommentsSection"] = commentsSection,
            ["@Model.DoctorName"] = WebUtility.HtmlEncode(physicianName),
            ["@Model.GeneratedAt"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            ["@Model.Year"] = DateTime.UtcNow.Year.ToString(CultureInfo.InvariantCulture),
        };
    }

    private static string BuildCommentsSection(IEnumerable<AnalysisComment> comments)
    {
        var items = comments.OrderBy(c => c.CreatedAt).ToList();
        if (items.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        sb.Append(
            $"""
             <div class="section">
                 <h2>Clinical notes &amp; comments ({items.Count})</h2>
                 <div class="comments-list">
             """);

        foreach (var comment in items)
        {
            sb.Append(
                $"""
                 <div class="comment-item">
                     <p class="comment-text">{WebUtility.HtmlEncode(comment.Content)}</p>
                     <p class="comment-meta">{comment.CreatedAt:yyyy-MM-dd HH:mm} UTC</p>
                 </div>
                 """);
        }

        sb.Append("</div></div>");
        return sb.ToString();
    }

    private static string BuildDetectionsTable(IEnumerable<DetectionFinding> detections)
    {
        var items = detections.ToList();
        if (items.Count == 0)
            return "<tr><td colspan='4'>No detections recorded.</td></tr>";

        var sb = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            var d = items[i];
            sb.Append("<tr>");
            sb.Append($"<td>{i + 1}</td>");
            sb.Append($"<td>{WebUtility.HtmlEncode(d.ClassName)}</td>");
            sb.Append($"<td>{d.Confidence.ToString("P1", CultureInfo.InvariantCulture)}</td>");
            sb.Append(
                $"<td>{d.XCenter:F3}, {d.YCenter:F3} ({d.Width:P1} × {d.Height:P1})</td>");
            sb.Append("</tr>");
        }

        return sb.ToString();
    }

    private static string BuildAnnotatedImageHtml(TumorAnalysis analysis)
    {
        var path = AnalysisImagePaths.ResolveFilePath(analysis, "annotated")
            ?? AnalysisImagePaths.ResolveFilePath(analysis, "detection")
            ?? AnalysisImagePaths.ResolveFilePath(analysis, "segmentation");

        if (path is null || !File.Exists(path))
            return "<p class='muted'>Annotated image not available.</p>";

        var fileInfo = new FileInfo(path);
        // Keep gRPC payload reasonable — large MRI embeds can exceed default limits and slow PDF rendering.
        if (fileInfo.Length > 1_500_000)
            return "<p class='muted'>Annotated image available in the portal (omitted from PDF for size).</p>";

        var bytes = File.ReadAllBytes(path);
        var b64 = Convert.ToBase64String(bytes);
        var ext = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
        var mime = ext switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "bmp" => "image/bmp",
            "webp" => "image/webp",
            _ => "image/png",
        };

        return $"""<img src="data:{mime};base64,{b64}" style="max-width:100%;border-radius:8px;border:1px solid #e5e7eb;" alt="Annotated scan" />""";
    }
}
