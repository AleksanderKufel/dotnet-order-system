using OrderSystem.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Application
{
    public class OrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMessagePublisher _publisher;

        public OrderService(IOrderRepository repository, IMessagePublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<Guid> CreateOrder(CreateOrderRequest request)
        {
            var order = new Order(request.CustomerEmail, request.Amount);
            await _repository.Add(order);
            await _publisher.PublishAsync(new OrderCreatedEvent(order.Id), "orders");

            return order.Id;
        }
    }
}
