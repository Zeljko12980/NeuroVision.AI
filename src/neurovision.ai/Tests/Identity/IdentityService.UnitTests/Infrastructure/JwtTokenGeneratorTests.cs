using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.UnitTests.Infrastructure;

public class JwtTokenGeneratorTests
{
    private const string Key = "super-secret-test-key-that-is-32b!";
    private const string Issuer = "identity-tests";
    private const string Audience = "neurovision-tests";

    [Fact]
    public void GenerateToken_ContainsExpectedClaims()
    {
        var userId = Guid.NewGuid();
        var generator = CreateGenerator();

        var token = generator.GenerateToken(userId, "jane@neurovision.ai", "doctor.jane", [RoleNames.Doctor]);

        var principal = Validate(token);
        principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value.Should().Be(userId.ToString());
        principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value.Should().Be("jane@neurovision.ai");
        principal.FindFirst(ClaimTypes.Name)?.Value.Should().Be("doctor.jane");
        principal.FindFirst(ClaimTypes.Role)?.Value.Should().Be(RoleNames.Doctor);
    }

    [Fact]
    public void GenerateToken_WhenKeyMissing_Throws()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        var generator = new JwtTokenGenerator(configuration, NullLogger<JwtTokenGenerator>.Instance);

        var act = () => generator.GenerateToken(Guid.NewGuid(), "a@b.c", "user", []);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Jwt:Key is not configured.");
    }

    private static JwtTokenGenerator CreateGenerator()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = Key,
                ["Jwt:Issuer"] = Issuer,
                ["Jwt:Audience"] = Audience,
                ["Jwt:ExpiryMinutes"] = "60"
            })
            .Build();

        return new JwtTokenGenerator(configuration, NullLogger<JwtTokenGenerator>.Instance);
    }

    private static ClaimsPrincipal Validate(string token)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        return handler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            ClockSkew = TimeSpan.Zero
        }, out _);
    }
}
