var builder = DistributedApplication.CreateBuilder(args);


//DATABASES

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();


var identityDb = postgres.AddDatabase("identitydb");


//MESSAGING
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin(port: 15672);

//MONITORING

var prometheus = builder.AddContainer("prometheus", "prom/prometheus")
     .WithHttpEndpoint(port: 9090, targetPort: 9090)
     .WithVolume("prometheus-data", "/prometheus");

var loki = builder.AddContainer("loki", "grafana/loki")
    .WithHttpEndpoint(port: 3100, targetPort: 3100)
    .WithVolume("loki-data", "/loki");

var grafana = builder.AddContainer("grafana", "grafana/grafana")
    .WithHttpEndpoint(port: 3000, targetPort: 3000)
    .WithVolume("grafana-data", "/var/lib/grafana");


//SERVICES


var identityService = builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(identityDb)
       .WithReference(identityDb)
       .WaitFor(loki)
       .WaitFor(prometheus)
       .WaitFor(grafana);


//GATEWAY

var gateway = builder.AddProject<Projects.Gateway_API>("gateway-api")
    .WaitFor(identityService)
    .WithReference(identityService);

//FRONTEND

builder.AddJavaScriptApp("portal", "../Clients/neurovision.ai.portal")
    .WaitFor(gateway);


builder.Build().Run();
