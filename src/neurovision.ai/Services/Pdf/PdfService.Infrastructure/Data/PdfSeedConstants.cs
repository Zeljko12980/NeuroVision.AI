namespace PdfService.Infrastructure.Data;

public static class PdfSeedConstants
{
    /// <summary>Stable ID referenced by TumorDetection PdfService:DefaultCertificateId.</summary>
    public static readonly Guid DefaultDoctorCertificateId =
        Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");

    public const string DefaultCertificateFileName = "neurovision-doctor-dev.pfx";

    public const string DefaultCertificatePassword = "NeuroVisionDev123!";

    public const string DefaultCertificateName = "NeuroVision Doctor (Dev)";

    public const string SignatureFileName = "signature.png";

    public const string EmailConfirmation = "EMAIL_CONFIRMATION";
    public const string EmailConfirmationMail = "EmailConfirmationTemplate";
    public const string SetPassword = "SET_PASSWORD";
    public const string SetPasswordMail = "SetPasswordTemplate";
    public const string SecurityCode = "SECURITY_CODE";
    public const string TumorAnalysisReport = "TUMOR_ANALYSIS_REPORT";
    public const string AccountCreated = "ACCOUNT_CREATED";
    public const string TwoFactor = "TwoFactorTemplate";
}
