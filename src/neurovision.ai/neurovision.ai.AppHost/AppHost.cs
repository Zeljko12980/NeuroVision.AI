var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Gateway_API>("gateway-api");

builder.Build().Run();
