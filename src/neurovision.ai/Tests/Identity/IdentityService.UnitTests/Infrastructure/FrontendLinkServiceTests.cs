using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityService.UnitTests.Infrastructure;

public class FrontendLinkServiceTests
{
    [Fact]
    public void BuildConfirmEmailLink_WhenFrontendUrlMissing_Fails()
    {
        var service = CreateService(null);

        var result = service.BuildConfirmEmailLink("user@neurovision.ai", "raw-token");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Frontend URL is not configured.");
    }

    [Fact]
    public void BuildConfirmEmailLink_EncodesEmailAndToken()
    {
        var service = CreateService("https://app.neurovision.ai/");

        var result = service.BuildConfirmEmailLink("user+test@neurovision.ai", "raw token");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().StartWith("https://app.neurovision.ai/confirm-email?");
        result.Value.Should().Contain("email=user%2Btest%40neurovision.ai");
        result.Value.Should().Contain("token=");
        result.Value.Should().NotContain("raw token");
    }

    [Fact]
    public void BuildSetPasswordLink_UsesSetPasswordPath()
    {
        var service = CreateService("https://app.neurovision.ai");

        var result = service.BuildSetPasswordLink("user@neurovision.ai", "token");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain("/set-password?");
        result.Value.Should().Contain("email=user%40neurovision.ai");
    }

    private static FrontendLinkService CreateService(string? frontendUrl)
    {
        var values = new Dictionary<string, string?>();
        if (frontendUrl is not null)
            values["AppSettings:FrontendUrl"] = frontendUrl;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        return new FrontendLinkService(configuration, NullLogger<FrontendLinkService>.Instance);
    }
}
