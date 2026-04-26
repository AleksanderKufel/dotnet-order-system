using OrderSystem.Application;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderSystem.Infrastructure
{
    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private IConnection? _connection;
        private IChannel? _channel;
        private readonly ConnectionFactory _factory;

        public RabbitMqPublisher()
        {
            _factory = new ConnectionFactory { HostName = "localhost" };
        }

        public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default)
        {
            if (_connection == null)
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
            }

            await _channel!.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body,
                cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}