using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Application
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default);
    }
}
