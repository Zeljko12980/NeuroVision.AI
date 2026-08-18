namespace MailService.Domain.Templates;

public static class EmailTemplates
{
    public static readonly EmailTemplate ConfirmEmail = EmailTemplate.Create(
        EmailTemplateCodes.EmailConfirmation,
        EmailTemplateCodes.EmailConfirmation,
        "Email Confirmation",
        "Please confirm your email using the attached PDF.",
        "EmailConfirmation.pdf",
        EmailPlaceholderKeys.FullName,
        EmailPlaceholderKeys.ConfirmationUrl);

    public static readonly EmailTemplate SetPassword = EmailTemplate.Create(
        EmailTemplateCodes.SetPassword,
        EmailTemplateCodes.SetPassword,
        "Set Your Password",
        "Please set your password using the attached PDF.",
        "SetPassword.pdf",
        EmailPlaceholderKeys.Email,
        EmailPlaceholderKeys.SetPasswordUrl);

    public static readonly EmailTemplate ForgotPassword = EmailTemplate.Create(
        EmailTemplateCodes.ForgotPassword,
        EmailTemplateCodes.SetPassword,
        "Reset Your Password",
        "Please reset your password using the attached PDF.",
        "ResetPassword.pdf",
        EmailPlaceholderKeys.Email,
        EmailPlaceholderKeys.SetPasswordUrl);

    public static readonly EmailTemplate TwoFactor = EmailTemplate.Create(
        EmailTemplateCodes.TwoFactor,
        EmailTemplateCodes.TwoFactor,
        "Your Two-Factor Authentication Code",
        "For security purposes, your two-factor authentication code has been sent as an attached PDF file. Please open the attachment to retrieve your code and complete your sign-in.",
        "TwoFactorCode.pdf",
        EmailPlaceholderKeys.FullName,
        EmailPlaceholderKeys.Code);

    public static EmailTemplate Get(string id)
    {
        if (TryGet(id, out var template))
            return template;

        throw new ArgumentException($"Unknown email template '{id}'.", nameof(id));
    }

    public static bool TryGet(string id, out EmailTemplate template)
    {
        template = id switch
        {
            EmailTemplateCodes.EmailConfirmation => ConfirmEmail,
            EmailTemplateCodes.SetPassword => SetPassword,
            EmailTemplateCodes.ForgotPassword => ForgotPassword,
            EmailTemplateCodes.TwoFactor => TwoFactor,
            _ => null!
        };

        return template is not null;
    }
}
