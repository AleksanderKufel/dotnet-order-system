using OrderSystem.Domain;

namespace OrderSystem.Application
{
    public interface IOrderRepository
    {
        Task Add(Order order);
    }
}