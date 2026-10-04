using OrderSystem.Domain;

namespace OrderSystem.Application
{
    public interface IOrderRepository
    {
        Task Add(Order order);
        Task<Order?> GetById(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken = default);
    }
}
