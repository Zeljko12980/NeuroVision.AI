using BuildingBlocks.Pagination;
using Gateway.API.Common.Interface;
using Gateway.API.Common.Model;
using Gateway.API.Service;
using Microsoft.Extensions.ServiceDiscovery;
using System.Net;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);


builder.AddServiceDefaults();


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
    });

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));



var frontendUrl = builder.Configuration["AppSettings:FrontendUrl"];


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(frontendUrl!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



builder.Services.AddHealthChecks();



var app = builder.Build();



app.UseCors("AllowFrontend");



app.MapDefaultEndpoints();



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