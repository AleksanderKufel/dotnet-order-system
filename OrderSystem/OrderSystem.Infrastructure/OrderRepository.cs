using Microsoft.EntityFrameworkCore;
using OrderSystem.Application;
using OrderSystem.Domain;

namespace OrderSystem.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task Add(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public Task<Order?> GetById(Guid id, CancellationToken cancellationToken = default) =>
            _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        public async Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken = default) =>
            await _context.Orders.AsNoTracking().ToListAsync(cancellationToken);
    }
}
