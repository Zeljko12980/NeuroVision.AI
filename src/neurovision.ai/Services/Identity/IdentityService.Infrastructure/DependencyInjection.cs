namespace IdentityService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            MappingConfig.RegisterMappings();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("identitydb")));

            var _key = configuration["Jwt:Key"];
            var _issuer = configuration["Jwt:Issuer"];
            var _audience = configuration["Jwt:Audience"];
            var _expirtyMinutes = configuration["Jwt:ExpiryMinutes"];

            services
           .AddAuthentication(x =>
           {
               x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
           }).AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidAudience = _audience,
                   ValidIssuer = _issuer,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
                   ClockSkew = TimeSpan.FromMinutes(Convert.ToDouble(_expirtyMinutes))
               };
           });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("PatientPolicy", policy =>
                    policy.RequireRole("Patient"));

                options.AddPolicy("DoctorPolicy", policy =>
                    policy.RequireRole("Doctor"));

                options.AddPolicy("SuperAdminPolicy", policy =>
                    policy.RequireRole("SuperAdministrator"));
            });

            services.AddIdentity<AspIdentityUser, AspIdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = true;
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IIdentityService, IdentityService.Infrastructure.Services.IdentityService>();

            return services;
        }
    }
}