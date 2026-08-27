// Visual Studio's Aspire debugger often returns HTTP 500 for Executable run
// requests, after which DCP falls back to `dotnet.exe` with no arguments.
// Clearing the debug-session variables forces process execution instead.
foreach (var key in new[]
{
    "DEBUG_SESSION_PORT",
    "DEBUG_SESSION_TOKEN",
    "DEBUG_SESSION_SERVER_CERTIFICATE",
    "DEBUG_SESSION_INFO",
    "DASHBOARD__DEBUGSESSION__PORT",
    "DASHBOARD__DEBUGSESSION__TOKEN",
    "DASHBOARD__DEBUGSESSION__SERVERCERTIFICATE"
})
{
    Environment.SetEnvironmentVariable(key, null);
}

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ProjectResource> WithJwt(IResourceBuilder<ProjectResource> resource)
{
    var key = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException(
            "Jwt:Key is not configured. Set Jwt:Key, Jwt:Issuer, and Jwt:Audience in AppHost user secrets.");

    return resource
        .WithEnvironment("Jwt__Key", key)
        .WithEnvironment("Jwt__Issuer", builder.Configuration["Jwt:Issuer"] ?? "jwt")
        .WithEnvironment("Jwt__Audience", builder.Configuration["Jwt:Audience"] ?? "jwt");
}


//DATABASES

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume()
                      .WithPgAdmin();


var identityDb = postgres.AddDatabase("identitydb");
var pdfDb = postgres.AddDatabase("pdfdb");
var locationDb = postgres.AddDatabase("locationdb");

var doctorDb = postgres.AddDatabase("doctordb");
var patientDb = postgres.AddDatabase("patientdb");
var notificationDb = postgres.AddDatabase("notificationdb");
var appointmentDb = postgres.AddDatabase("appointmentdb");
var tumorDetectionDb = postgres.AddDatabase("tumordetectiondb");


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

var grafanaAdminUser = builder.Configuration["Grafana:AdminUser"] ?? "admin";
var grafanaAdminPassword = builder.Configuration["Grafana:AdminPassword"] ?? "admin";

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


var identityService = WithJwt(builder.AddProject<Projects.IdentityService_API>("identityservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(identityDb)
       .WithReference(identityDb)
       .WaitFor(loki)
       .WithEnvironment("Observability__ServiceName", "identityservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"))
       .WithEnvironment("IdentitySeed__Patient__Id", "22222222-2222-2222-2222-222222222222")
       .WithEnvironment("IdentitySeed__Patient__Email", "armanigas78@gmail.com")
       .WithEnvironment("IdentitySeed__Patient__UserName", "armanigas78")
       .WithEnvironment("IdentitySeed__Patient__Password", "Patient123!")
       .WithEnvironment("IdentitySeed__Doctor__Id", "a1a1a1a1-a1a1-a1a1-a1a1-a1a1a1a1a1a1"));

var pdfService = WithJwt(builder.AddProject<Projects.PdfService_API>("pdfservice-api")
       .WaitFor(pdfDb)
       .WithReference(pdfDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6002, isProxied: false)
       .WithHttpEndpoint(name: "grpc", port: 6102, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "pdfservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"))
       .WithEnvironment("Kestrel__Endpoints__http__Protocols", "Http1")
       .WithEnvironment("Kestrel__Endpoints__grpc__Protocols", "Http2"));

var locationService = WithJwt(builder.AddProject<Projects.LocationService_API>("locationservice-api")
       .WaitFor(locationDb)
       .WithReference(locationDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6003, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "locationservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http")));

builder.AddProject<Projects.MailService_API>("mailservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(pdfService)
       .WithReference(pdfService)
       .WaitFor(loki)
       .WithEnvironment("Observability__ServiceName", "mailservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"))
       .WithEnvironment("PdfService__GrpcUrl", pdfService.GetEndpoint("grpc"));

var doctorService = WithJwt(builder.AddProject<Projects.DoctorService_API>("doctorservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(doctorDb)
       .WithReference(doctorDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6004, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "doctorservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http")));

var patientService = WithJwt(builder.AddProject<Projects.PatientService_API>("patientservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(patientDb)
       .WithReference(patientDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6005, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "patientservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http")));

var notificationService = WithJwt(builder.AddProject<Projects.NotificationService_API>("notificationservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(notificationDb)
       .WithReference(notificationDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6006, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "notificationservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http")));

var appointmentService = WithJwt(builder.AddProject<Projects.AppointmentService_API>("appointmentservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(appointmentDb)
       .WithReference(appointmentDb)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6007, isProxied: false)
       .WithEnvironment("Observability__ServiceName", "appointmentservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http")));

var tumorDetectionService = WithJwt(builder.AddProject<Projects.TumorDetectionService_API>("tumordetectionservice-api")
       .WaitFor(rabbitmq)
       .WithReference(rabbitmq)
       .WaitFor(tumorDetectionDb)
       .WithReference(tumorDetectionDb)
       .WithReference(pdfService)
       .WaitFor(loki)
       .WithHttpEndpoint(name: "http", port: 6008, isProxied: false)
       .WithHttpHealthCheck("/health")
       .WithEnvironment("Observability__ServiceName", "tumordetectionservice-api")
       .WithEnvironment("Observability__LokiUrl", loki.GetEndpoint("http"))
       .WithEnvironment("PdfService__GrpcUrl", pdfService.GetEndpoint("grpc")));


//GATEWAY

var gateway = builder.AddProject<Projects.Gateway_API>("gateway-api")
    .WaitFor(identityService)
    .WithReference(identityService)
    .WaitFor(pdfService)
    .WithReference(pdfService)
    .WaitFor(locationService)
    .WithReference(locationService)
    .WaitFor(doctorService)
    .WithReference(doctorService)
    .WaitFor(patientService)
    .WithReference(patientService)
    .WaitFor(notificationService)
    .WithReference(notificationService)
    .WaitFor(appointmentService)
    .WithReference(appointmentService)
    .WaitFor(tumorDetectionService)
    .WithReference(tumorDetectionService)
    .WithHttpEndpoint(name: "http", port: 5000, isProxied: false)
    .WithHttpsEndpoint(name: "https", port: 5050, isProxied: false)
    .WithEnvironment("Services__identity", "https://localhost:6060/health")
    .WithEnvironment("Services__pdf", "http://localhost:6002/health")
    .WithEnvironment("Services__location", "http://localhost:6003/health")
    .WithEnvironment("Services__doctor", "http://localhost:6004/health")
    .WithEnvironment("Services__patient", "http://localhost:6005/health")
    .WithEnvironment("Services__notification", "http://localhost:6006/health")
    .WithEnvironment("Services__appointment", "http://localhost:6007/health")
    .WithEnvironment("Services__tumor", "http://localhost:6008/health");

//FRONTEND

builder.AddJavaScriptApp("portal", "../Clients/neurovision.ai.portal")
    .WaitFor(gateway);


builder.Build().Run();
