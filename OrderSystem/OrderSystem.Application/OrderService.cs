using OrderSystem.Domain;

namespace OrderSystem.Application
{
    public class OrderService
    {
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        private readonly IOrderRepository _repository;
        private readonly IMessagePublisher _publisher;
        private readonly ICacheService _cache;

        public OrderService(IOrderRepository repository, IMessagePublisher publisher, ICacheService cache)
        {
            _repository = repository;
            _publisher = publisher;
            _cache = cache;
        }

        public async Task<Guid> CreateOrder(CreateOrderRequest request)
        {
            var order = new Order(request.CustomerEmail, request.Amount);
            await _repository.Add(order);
            await _publisher.PublishAsync(new OrderCreatedEvent(order.Id), "orders");

            return order.Id;
        }

        public async Task<OrderDto?> GetOrder(Guid id, CancellationToken cancellationToken = default)
        {
            var cacheKey = OrderCacheKeys.ForOrder(id);

            var cached = await _cache.GetAsync<OrderDto>(cacheKey);
            if (cached is not null)
                return cached;

            var order = await _repository.GetById(id, cancellationToken);
            if (order is null)
                return null;

            var dto = OrderDto.FromOrder(order);
            await _cache.SetAsync(cacheKey, dto, CacheExpiry);

            return dto;
        }

        public async Task<IReadOnlyList<OrderDto>> GetOrders(CancellationToken cancellationToken = default)
        {
            var orders = await _repository.GetAll(cancellationToken);
            return orders.Select(OrderDto.FromOrder).ToList();
        }
    }
}
