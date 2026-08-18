namespace MailService.Domain.Constants;

public static class EmailPlaceholderKeys
{
    public const string FullName = "@Model.FullName";
    public const string Email = "@Model.Email";
    public const string ConfirmationUrl = "@Model.ConfirmationUrl";
    public const string SetPasswordUrl = "@Model.SetPasswordUrl";
    public const string Code = "@Model.Code";
}
