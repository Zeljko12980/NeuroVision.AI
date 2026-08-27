var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Host.AddSerilogObservability();
builder.Services.AddObservabilityTelemetry(builder.Configuration);
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 536_870_912;
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 536_870_912;
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddSingleton<CustomExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration, TumorAnalysisHubRoutes.PathPrefix);
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, JwtUserIdProvider>();
builder.Services.AddSingleton<IAnalysisNotificationPublisher, AnalysisNotificationPublisher>();
var frontendUrl = builder.Configuration.GetValue<string>("AppSettings:FrontendUrl")
    ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseStaticFiles();
app.MapDefaultEndpoints();

await TumorDetectionDbInitializer.InitializeAsync(app.Services);

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReferenceIfAvailable("Tumor Detection Service API");
}

app.UseWebSockets();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<TumorAnalysisHub>(TumorAnalysisHubRoutes.Analysis);
app.MapControllers();

app.Run();
