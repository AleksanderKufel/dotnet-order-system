using OrderSystem.Application;
using OrderSystem.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderSystem.Worker
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;

        public OrderCreatedConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _factory = new ConnectionFactory { HostName = "localhost" };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: "orders",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

                    if (orderEvent != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                        var order = await db.Orders.FindAsync(new object[] { orderEvent.OrderId }, stoppingToken);

                        if (order is not null)
                        {
                            await Task.Delay(1000, stoppingToken); // Work simulation
                            order.MarkAsProcessed();
                            await db.SaveChangesAsync(stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" [!] Message processing failed: {ex.Message}");
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "orders",
                autoAck: true, 
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync(cancellationToken);
            if (_connection != null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}