var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Host.AddSerilogObservability();
builder.Services.AddObservabilityTelemetry(builder.Configuration);

builder.Services.AddSingleton<CustomExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddApplicationServices(builder.Configuration);


var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspIdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AspIdentityRole>>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
    var seedOptions = app.Configuration
        .GetSection(IdentitySeedOptions.SectionName)
        .Get<IdentitySeedOptions>();

    await DatabaseSeeder.SeedAsync(userManager, roleManager, seedOptions, seedLogger);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReferenceIfAvailable("Identity Service API");
}


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
