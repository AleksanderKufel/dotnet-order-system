using OrderSystem.Application;
using OrderSystem.Domain;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
