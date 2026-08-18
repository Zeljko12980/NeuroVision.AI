

using PdfService.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddGrpcClient<PdfGenerator.PdfGeneratorClient>(options =>
{
    options.Address = new Uri(builder.Configuration["PdfService:GrpcUrl"]!);
});

builder.Services.AddScoped<IPdfServiceClient, PdfServiceClient>();
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
