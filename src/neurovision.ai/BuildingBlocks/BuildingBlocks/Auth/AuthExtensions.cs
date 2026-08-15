using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.FeatureManagement.FeatureFilters;
using System.IdentityModel.Tokens.Jwt;

namespace BuildingBlocks.Auth
{
    public static class AuthExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var _key = configuration["Jwt:Key"];
            var _issuer = configuration["Jwt:Issuer"];
            var _audience = configuration["Jwt:Audience"];
            var _expiryMinutes = configuration["Jwt:ExpiryMinutes"];

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = _issuer,
                    ValidAudience = _audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_key))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("SuperAdministrator"));

                options.AddPolicy("DoctorPolicy", policy =>
                    policy.RequireRole("Doctor"));

                options.AddPolicy("PatientPolicy", policy =>
                    policy.RequireRole("Patient"));
            });

            return services;
        }
    }
}
