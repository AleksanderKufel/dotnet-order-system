using OrderSystem.Application;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure;
using OrderSystem.Domain;

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

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext db, RedisCacheService cache) =>
{
    string cacheKey = $"order:{id}";

    // Try to get the order from cache first
    var cachedOrder = await cache.GetAsync<Order>(cacheKey);
    if (cachedOrder is not null)
    {
        return Results.Ok(cachedOrder);
    }

    // If not in cache, get it from the database
    var order = await db.Orders.FindAsync(id);

    if (order is null)
    {
        return Results.NotFound();
    }

    // Save the order to cache for future requests
    await cache.SetAsync(cacheKey, order, TimeSpan.FromMinutes(10));

    return Results.Ok(order);
});

app.MapGet("/orders", async (OrderDbContext db) =>
{
    var orders = await db.Orders.ToListAsync();
    return Results.Ok(orders);
});

app.Run();