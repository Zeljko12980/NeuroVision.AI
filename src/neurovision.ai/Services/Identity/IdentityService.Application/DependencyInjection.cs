namespace IdentityService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            MappingConfig.RegisterMappings();

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(ctg =>
            {
                ctg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                ctg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                ctg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            return services;
        }
    }
}
