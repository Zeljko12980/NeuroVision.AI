using LocationService.Application;
using LocationService.Infrastructure;
using LocationService.Infrastructure.Persistence;
using LocationService.Infrastructure.Seeding;
using Scalar.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);


var app = builder.Build();


app.MapDefaultEndpoints();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


app.UseHttpsRedirection();


app.MapControllers();


await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<LocationDbContext>();

    await context.Database.MigrateAsync();

    await context.SeedAsync();
}


app.Run();