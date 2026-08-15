namespace IdentityService.Infrastructure.Services;

public class FrontendLinkService : IFrontendLinkService
{
    private readonly string? _frontendUrl;
    private readonly ILogger<FrontendLinkService> _logger;

    public FrontendLinkService(IConfiguration configuration, ILogger<FrontendLinkService> logger)
    {
        _frontendUrl = configuration["AppSettings:FrontendUrl"];
        _logger = logger;
    }

    public Result<string> BuildConfirmEmailLink(string email, string rawToken)
        => BuildLink("/confirm-email", email, rawToken);

    public Result<string> BuildSetPasswordLink(string email, string rawToken)
        => BuildLink("/set-password", email, rawToken);

    private Result<string> BuildLink(string path, string email, string rawToken)
    {
        if (string.IsNullOrWhiteSpace(_frontendUrl))
        {
            _logger.LogError("Frontend URL is not configured. Path={Path}, Email={Email}", path, email);
            return Result<string>.Fail("Frontend URL is not configured.");
        }

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
        var encodedEmail = Uri.EscapeDataString(email);

        _logger.LogDebug("Frontend link built. Path={Path}, Email={Email}", path, email);

        return Result<string>.Ok($"{_frontendUrl.TrimEnd('/')}{path}?email={encodedEmail}&token={encodedToken}");
    }
}
