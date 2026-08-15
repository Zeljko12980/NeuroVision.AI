namespace IdentityService.Application.Common.Interfaces;

public interface IFrontendLinkService
{
    Result<string> BuildConfirmEmailLink(string email, string rawToken);
    Result<string> BuildSetPasswordLink(string email, string rawToken);
}
