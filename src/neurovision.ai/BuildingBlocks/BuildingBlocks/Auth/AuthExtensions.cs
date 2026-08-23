using System.Security.Claims;

namespace BuildingBlocks.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        params string[] signalRHubPathPrefixes)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = configuration["Jwt:Issuer"] ?? "jwt";
        var audience = configuration["Jwt:Audience"] ?? "jwt";

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name
                };

                if (signalRHubPathPrefixes.Length == 0)
                    return;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (string.IsNullOrEmpty(accessToken))
                            return Task.CompletedTask;

                        var path = context.HttpContext.Request.Path;
                        if (signalRHubPathPrefixes.Any(prefix =>
                            path.StartsWithSegments(prefix, StringComparison.OrdinalIgnoreCase)))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.Patient, policy =>
                policy.RequireRole(AuthRoles.Patient));

            options.AddPolicy(AuthPolicies.Doctor, policy =>
                policy.RequireRole(AuthRoles.Doctor));

            options.AddPolicy(AuthPolicies.SuperAdmin, policy =>
                policy.RequireRole(AuthRoles.SuperAdministrator));

            options.AddPolicy(AuthPolicies.Staff, policy =>
                policy.RequireRole(AuthRoles.SuperAdministrator, AuthRoles.Doctor));
        });

        return services;
    }
}
