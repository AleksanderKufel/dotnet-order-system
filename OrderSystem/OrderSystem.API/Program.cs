using Microsoft.EntityFrameworkCore;
using OrderSystem.API;
using OrderSystem.Application;
using OrderSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddScoped<OrderService>();

builder.Services.AddValidation();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Convenience for local and Docker runs only. In production, migrations run as a separate deployment step.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.MigrateAsync();
}

app.MapOrderEndpoints();

app.Run();
