var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();


var identityDb = postgres.AddDatabase("identitydb");


builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(identityDb)
       .WithReference(identityDb);


builder.Build().Run();
