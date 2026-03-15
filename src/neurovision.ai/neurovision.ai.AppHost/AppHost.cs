var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin(port: 15672);

var identityDb = postgres.AddDatabase("identitydb");


builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(identityDb)
       .WithReference(identityDb);


builder.AddProject<Projects.MailService_API>("mailservice-api")
         .WaitFor(rabbitmq)
         .WithReference(rabbitmq);


builder.Build().Run();
