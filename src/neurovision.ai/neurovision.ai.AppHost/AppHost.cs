var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.IdentityService_API>("identityservice-api");

builder.Build().Run();
