var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin(port: 15672);

var identityDb = postgres.AddDatabase("identitydb");


var identityService = builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(identityDb)
       .WithReference(identityDb);


builder.AddProject<Projects.MailService_API>("mailservice-api")
         .WaitFor(rabbitmq)
         .WithReference(rabbitmq);

var gateway = builder.AddYarp("gateway")
                     .WithHostPort(5000)
                     .WithConfiguration(yarp =>
                     {

                         yarp.AddRoute("/api/Authentication/{**catch-all}", identityService);
                     });

builder.AddJavaScriptApp("portal", "../Client/neurovision.ai.portal")
    .WaitFor(gateway);




builder.Build().Run();
