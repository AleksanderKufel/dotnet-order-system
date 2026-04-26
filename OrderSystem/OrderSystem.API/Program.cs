using OrderSystem.Application;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

app.MapPost("/orders", async (CreateOrderRequest request, OrderService service) =>
{
    var id = await service.CreateOrder(request);
    return Results.Ok(id);
});

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext db) =>
{
    var order = await db.Orders.FindAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
});

app.MapGet("/orders", async (OrderDbContext db) =>
{
    var orders = await db.Orders.ToListAsync();
    return Results.Ok(orders);
});

app.Run();