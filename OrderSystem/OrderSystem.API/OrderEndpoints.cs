using Microsoft.AspNetCore.Http.HttpResults;
using OrderSystem.Application;

namespace OrderSystem.API
{
    public static class OrderEndpoints
    {
        private const string GetOrderRouteName = "GetOrder";

        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/orders");

            group.MapPost("/", async (CreateOrderRequest request, OrderService service) =>
            {
                var id = await service.CreateOrder(request);
                return TypedResults.CreatedAtRoute(id, GetOrderRouteName, new { id });
            });

            group.MapGet("/{id:guid}", async Task<Results<Ok<OrderDto>, NotFound>> (
                Guid id, OrderService service, CancellationToken cancellationToken) =>
            {
                var order = await service.GetOrder(id, cancellationToken);
                return order is null ? TypedResults.NotFound() : TypedResults.Ok(order);
            })
            .WithName(GetOrderRouteName);

            group.MapGet("/", async (OrderService service, CancellationToken cancellationToken) =>
                TypedResults.Ok(await service.GetOrders(cancellationToken)));

            return app;
        }
    }
}
