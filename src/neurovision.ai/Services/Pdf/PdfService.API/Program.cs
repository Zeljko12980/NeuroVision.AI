var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.AddSerilogObservability();
builder.Services.AddObservabilityTelemetry(builder.Configuration);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddGrpc();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var frontendUrl = builder.Configuration["AppSettings:FrontendUrl"];

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

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("PDF Service API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseCors("AllowFrontend");

app.MapControllers();

app.MapGrpcService<PdfGeneratorService>();

await PdfDbinitializer.InitializeAsync(app.Services);

app.Run();