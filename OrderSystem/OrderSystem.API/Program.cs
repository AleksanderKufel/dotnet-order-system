using OrderSystem.Application;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Application
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Convenience for local and Docker runs only. In production, migrations run as a separate deployment step.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await db.Database.MigrateAsync();
}

app.MapPost("/orders", async (CreateOrderRequest request, OrderService service) =>
{
    var id = await service.CreateOrder(request);
    return Results.Ok(id);
});

app.MapGet("/orders/{id}", async (Guid id, OrderDbContext db, RedisCacheService cache) =>
{
    string cacheKey = $"order:{id}";

    // Try to get the order from cache first
    var cachedOrder = await cache.GetAsync<OrderDto>(cacheKey);
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

    var dto = new OrderDto(
        order.Id,
        order.CustomerEmail,
        order.Amount,
        order.Status.ToString()
    );

    // Save the order to cache for future requests
    await cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

    return Results.Ok(dto);
});

app.MapGet("/orders", async (OrderDbContext db) =>
{
    var orders = await db.Orders.ToListAsync();
    return Results.Ok(orders);
});

app.Run();