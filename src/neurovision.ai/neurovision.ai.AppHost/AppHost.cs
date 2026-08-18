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
     .WithHttpEndpoint(name: "http", port: 9090, targetPort: 9090)
     .WithBindMount(
         Path.Combine(builder.AppHostDirectory, "monitoring", "prometheus", "prometheus.yml"),
         "/etc/prometheus/prometheus.yml")
     .WithArgs("--config.file=/etc/prometheus/prometheus.yml", "--storage.tsdb.path=/prometheus")
     .WithVolume("prometheus-data", "/prometheus");

var loki = builder.AddContainer("loki", "grafana/loki")
    .WithHttpEndpoint(name: "http", port: 3100, targetPort: 3100)
    .WithVolume("loki-data", "/loki");

var grafanaAdminUser = builder.Configuration["Grafana:AdminUser"]
    ?? throw new InvalidOperationException("Grafana:AdminUser is required.");
var grafanaAdminPassword = builder.Configuration["Grafana:AdminPassword"]
    ?? throw new InvalidOperationException("Grafana:AdminPassword is required.");

var grafana = builder.AddContainer("grafana", "grafana/grafana:11.6.3")
    .WithHttpEndpoint(name: "http", port: 3000, targetPort: 3000)
    .WithVolume("grafana-nv-data", "/var/lib/grafana")
    .WithBindMount(
        Path.Combine(builder.AppHostDirectory, "monitoring", "grafana", "provisioning", "datasources"),
        "/etc/grafana/provisioning/datasources")
    .WithBindMount(
        Path.Combine(builder.AppHostDirectory, "monitoring", "grafana", "provisioning", "dashboards"),
        "/etc/grafana/provisioning/dashboards")
    .WithEnvironment("GF_SECURITY_ADMIN_USER", grafanaAdminUser)
    .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", grafanaAdminPassword)
    .WithEnvironment("GF_USERS_ALLOW_SIGN_UP", "false")
    .WithEnvironment("GF_SECURITY_ALLOW_EMBEDDING", "true")
    .WithEnvironment("GF_SECURITY_COOKIE_SAMESITE", "disabled")
    .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
    .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Viewer")
    .WithEnvironment("GF_SERVER_ROOT_URL", "http://localhost:3000")
    .WaitFor(prometheus)
    .WaitFor(loki);


//SERVICES


var identityService = builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(identityDb)
       .WithReference(identityDb)
       .WaitFor(loki)
       .WaitFor(prometheus)
       .WaitFor(grafana)
       .WithEnvironment("Observability__ServiceName", "identityservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"));

builder.AddProject<Projects.MailService_API>("mailservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(loki)
       .WaitFor(prometheus)
       .WithEnvironment("Observability__ServiceName", "mailservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"));


//GATEWAY

var gateway = builder.AddProject<Projects.Gateway_API>("gateway-api")
    .WaitFor(identityService)
    .WithReference(identityService);

//FRONTEND

builder.AddJavaScriptApp("portal", "../Clients/neurovision.ai.portal")
    .WaitFor(gateway)
    .WithEnvironment("VITE_GRAFANA_URL", "http://localhost:3000");


builder.Build().Run();
