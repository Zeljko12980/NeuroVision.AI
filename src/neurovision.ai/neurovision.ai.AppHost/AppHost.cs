var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();


var identityDb = postgres.AddDatabase("identitydb");


builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(identityDb)
       .WithReference(identityDb);


builder.AddProject<Projects.DoctorService_API>("doctorservice-api");


builder.AddProject<Projects.MailService_API>("mailservice-api");


builder.Build().Run();
