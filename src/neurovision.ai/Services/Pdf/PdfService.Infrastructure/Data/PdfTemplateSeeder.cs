using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Data;

internal static class PdfTemplateSeeder
{
    private const int SeedVersion = 4;

    public static async Task SeedAsync(
        PdfDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var seeds = CreateSeeds();
        var existing = await db.Templates.ToListAsync(cancellationToken);
        var byCode = existing.ToDictionary(t => t.Code, StringComparer.OrdinalIgnoreCase);
        var added = 0;
        var updated = 0;

        foreach (var seed in seeds)
        {
            if (!byCode.TryGetValue(seed.Code, out var template))
            {
                await db.Templates.AddAsync(seed, cancellationToken);
                byCode[seed.Code] = seed;
                added++;
                continue;
            }

            template.Update(seed.Name, seed.HtmlContent, SeedVersion, seed.IsActive);
            updated++;
        }

        if (added > 0 || updated > 0)
            await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "PDF templates seeded. Added={Added}, Updated={Updated}, Codes={Codes}",
            added,
            updated,
            string.Join(", ", seeds.Select(s => s.Code)));
    }

    private static List<PdfTemplate> CreateSeeds()
    {
        var emailConfirmationHtml = PdfBrandLayout.Document(
            "Confirm your email",
            "Secure account activation",
            """
            <p class="text">Hello <span class="highlight">@Model.FullName</span>,</p>
            <p class="text">Confirm your email address to activate your NeuroVision.AI account.</p>
            <p style="text-align:center;margin:24px 0;">
                <a href="@Model.ConfirmationUrl" class="button">Confirm email</a>
            </p>
            <p class="info">This link expires in <strong>10 minutes</strong>.</p>
            <div class="warning">If you did not create this account, you can ignore this document.</div>
            """);

        var setPasswordHtml = PdfBrandLayout.Document(
            "Set your password",
            "Secure account access",
            """
            <p class="text">Hello <span class="highlight">@Model.Email</span>,</p>
            <p class="text">Choose a password to finish setting up your NeuroVision.AI account.</p>
            <p style="text-align:center;margin:24px 0;">
                <a href="@Model.SetPasswordUrl" class="button">Set password</a>
            </p>
            <p class="info">This link expires in <strong>10 minutes</strong>.</p>
            <div class="warning">If you did not request this, contact support and do not use the link.</div>
            """);

        var securityCodeHtml = PdfBrandLayout.Document(
            "Verification code",
            "Two-factor authentication",
            """
            <p class="text">Hello <span class="highlight">@Model.FullName</span>,</p>
            <p class="text">We received a request to access your account. Use the verification code below to continue.</p>
            <div class="code-box">
                <div class="code">@Model.Code</div>
                <div class="code-hint">Valid for 10 minutes</div>
            </div>
            <div class="warning"><strong>Security notice:</strong> Never share this code. NeuroVision.AI will never ask for it.</div>
            """);

        var accountCreatedHtml = PdfBrandLayout.Document(
            "Account created",
            "Welcome to NeuroVision.AI",
            """
            <p class="text">Dear <span class="highlight">@Model.FullName</span>,</p>
            <p class="text">Your NeuroVision.AI account is ready. Keep these credentials private and change your password after the first login.</p>
            <div class="panel">
                <p><strong>Email:</strong> @Model.Email</p>
                <p><strong>Username:</strong> @Model.Username</p>
                <p><strong>Password:</strong> @Model.Password</p>
            </div>
            <div class="warning"><strong>Security notice:</strong> Do not share this document. Contact support if you did not expect this account.</div>
            """);

        var tumorReport = PdfTemplate.Create(
            PdfSeedConstants.TumorAnalysisReport,
            "Tumor Analysis Report",
            PdfBrandLayout.Document(
                "@Model.ReportTitle",
                "AI-assisted neuroimaging diagnostics",
                """
                <table class="meta" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="meta-item"><label>Analysis ID</label><span>@Model.AnalysisId</span></td>
                        <td class="meta-item"><label>Status</label><span>@Model.AnalysisStatus</span></td>
                    </tr>
                    <tr>
                        <td class="meta-item"><label>Scan file</label><span>@Model.ScanFileName</span></td>
                        <td class="meta-item"><label>Scan type</label><span>@Model.ScanType</span></td>
                    </tr>
                    <tr>
                        <td class="meta-item"><label>Requested</label><span>@Model.RequestedAt</span></td>
                        <td class="meta-item"><label>Completed</label><span>@Model.CompletedAt</span></td>
                    </tr>
                    <tr>
                        <td class="meta-item"><label>Classification</label><span>@Model.ClassificationClass</span></td>
                        <td class="meta-item"><label>Confidence</label><span>@Model.ClassificationConfidence</span></td>
                    </tr>
                    <tr>
                        <td class="meta-item"><label>Tumor area</label><span>@Model.TumorAreaRatio</span></td>
                        <td class="meta-item"><label>Overall confidence</label><span>@Model.OverallConfidence</span></td>
                    </tr>
                </table>
                <div class="section">
                    <h2>Detections (@Model.DetectionCount)</h2>
                    <table class="data">
                        <thead>
                            <tr><th>#</th><th>Class</th><th>Confidence</th><th>Bounding box</th></tr>
                        </thead>
                        <tbody>@Model.DetectionsTable</tbody>
                    </table>
                </div>
                <div class="section">
                    <h2>Annotated scan</h2>
                    <div class="image-box">@Model.AnnotatedImage</div>
                </div>
                @Model.CorrectionSection
                @Model.CommentsSection
                <div class="section signature-section">
                    <h2>Physician approval</h2>
                    <p class="doctor-name">@Model.DoctorName</p>
                    <p class="doctor-role">Attending physician · NeuroVision.AI</p>
                    <div class="signature-visual">{{Signature}}</div>
                    <p class="info">Digitally signed medical report</p>
                </div>
                <p class="info" style="margin-top:24px;">Generated @Model.GeneratedAt UTC · © @Model.Year NeuroVision.AI</p>
                """,
                extraCss: """
                    .meta { margin-bottom: 8px; }
                    .meta-item { width: 50%; padding: 8px; vertical-align: top; }
                    .meta-item label { display: block; font-size: 11px; text-transform: uppercase; letter-spacing: .04em; color: #667085; margin-bottom: 4px; }
                    .meta-item span { display: block; padding: 10px 12px; background: #f9fafb; border: 1px solid #e4e7ec; border-radius: 8px; font-size: 13px; font-weight: 600; }
                    .section { margin-top: 22px; }
                    .section h2 { font-size: 15px; color: #3641f5; border-bottom: 2px solid #dde9ff; padding-bottom: 6px; margin: 0 0 12px; }
                    table.data { width: 100%; border-collapse: collapse; font-size: 13px; }
                    table.data th, table.data td { border: 1px solid #e4e7ec; padding: 8px 10px; text-align: left; }
                    table.data th { background: #ecf3ff; color: #262e89; }
                    .image-box { margin-top: 8px; text-align: center; background: #f9fafb; border: 1px dashed #d0d5dd; border-radius: 8px; padding: 12px; }
                    .signature-section { margin-top: 28px; padding-top: 16px; border-top: 2px solid #dde9ff; }
                    .doctor-name { font-size: 15px; font-weight: 700; margin: 0 0 4px; color: #101828; }
                    .doctor-role { font-size: 12px; color: #667085; margin: 0 0 12px; }
                    .signature-visual { min-height: 64px; }
                    """,
                wide: true),
            version: SeedVersion,
            requiresSignature: true);

        return
        [
            Mail(PdfSeedConstants.EmailConfirmation, "Email Confirmation", emailConfirmationHtml),
            Mail(PdfSeedConstants.EmailConfirmationMail, "Email Confirmation", emailConfirmationHtml),
            Mail(PdfSeedConstants.SetPassword, "Set Password", setPasswordHtml),
            Mail(PdfSeedConstants.SetPasswordMail, "Set Password", setPasswordHtml),
            Mail(PdfSeedConstants.SecurityCode, "Security Code", securityCodeHtml),
            Mail(PdfSeedConstants.TwoFactor, "Two Factor Authentication Code", securityCodeHtml),
            Mail(PdfSeedConstants.AccountCreated, "Account Created", accountCreatedHtml),
            tumorReport
        ];
    }

    private static PdfTemplate Mail(string code, string name, string html) =>
        PdfTemplate.Create(code, name, html, version: SeedVersion);
}
