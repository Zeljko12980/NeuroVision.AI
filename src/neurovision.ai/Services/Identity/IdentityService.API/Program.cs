using BuildingBlocks.Logging;
using Microsoft.AspNetCore.Identity;
using IdentityService.Infrastructure.Persistence;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();

var lokiUrl = builder.Configuration.GetValue<string>("AppSettings:Loki")
    ?? throw new InvalidOperationException("Loki URL is not configured.");

var serviceName = builder.Configuration.GetValue<string>("AppSettings:ServiceName")
    ?? builder.Environment.ApplicationName;

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Host.AddSerilogObservability();
builder.Services.AddObservabilityTelemetry(builder.Configuration);

builder.Services.AddSingleton<CustomExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var frontendUrl = builder.Configuration.GetValue<string>("AppSettings:FrontendUrl");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    db.Database.Migrate();

     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AspIdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AspIdentityRole>>();
    await DatabaseSeeder.SeedAsync(db, userManager, roleManager);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();