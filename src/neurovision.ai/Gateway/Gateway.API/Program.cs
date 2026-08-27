using BuildingBlocks.Pagination;
using Gateway.API.Common.Interface;
using Gateway.API.Common.Model;
using Gateway.API.Service;
using Microsoft.Extensions.ServiceDiscovery;
using System.Net;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 536_870_912;
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 536_870_912;
});


builder.Services.Configure<ServiceEndpointsOptions>(options =>
{
    options.Endpoints =
        builder.Configuration
            .GetSection("Services")
            .Get<Dictionary<string, string>>()
        ?? new();
});



builder.Services.AddHttpClient<IServiceHealthChecker, ServiceHealthChecker>();



// Add named HttpClient for YARP with service discovery
builder.Services.AddHttpClient("yarp-forwarder")
    .AddServiceDiscovery()
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        UseProxy = false,
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.None,
        UseCookies = false,
        ActivityHeadersPropagator = DistributedContextPropagator.Current,
        ConnectTimeout = TimeSpan.FromSeconds(15),
    })
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(10);
    });

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));



var frontendUrl = builder.Configuration["AppSettings:FrontendUrl"];
if (string.IsNullOrWhiteSpace(frontendUrl))
{
    throw new InvalidOperationException(
        "AppSettings:FrontendUrl is not configured. Set it to the portal origin (e.g. http://localhost:5173).");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



builder.Services.AddHealthChecks();
builder.Services.AddOpenApi();



var app = builder.Build();



app.UseCors("AllowFrontend");
app.UseWebSockets();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReferenceIfAvailable("Gateway API");
}

app.MapReverseProxy();



app.MapGet("/api/system/health",
async (
    [AsParameters] PaginationRequest request,
    IServiceHealthChecker checker) =>
{
    var result = await checker.CheckAsync(request);

    return Results.Ok(result);
});



app.Run();