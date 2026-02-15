namespace IdentityService.Application
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            MappingConfig.RegisterMappings();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            return services;
        }
    }
}
