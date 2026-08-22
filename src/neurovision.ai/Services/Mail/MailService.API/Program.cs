var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.AddSerilogObservability();
builder.Services.AddObservabilityTelemetry(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Mail Service API")
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.Run();

public partial class Program;
