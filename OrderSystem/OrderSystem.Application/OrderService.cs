using OrderSystem.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Application
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> CreateOrder(CreateOrderRequest request)
        {
            var order = new Order(request.CustomerEmail, request.Amount);
            await _repository.Add(order);
            return order.Id;
        }
    }
}
