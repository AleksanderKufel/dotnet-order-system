using OrderSystem.Domain;

namespace OrderSystem.Application
{
    public record OrderDto(Guid Id, string CustomerEmail, decimal Amount, string Status)
    {
        public static OrderDto FromOrder(Order order) =>
            new(order.Id, order.CustomerEmail, order.Amount, order.Status.ToString());
    }
}
