using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;

namespace OrderSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<OrderDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("Default")));

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

            // Use lazy loading to prevent the application from crashing on startup if Redis is not ready. 
            // The connection will be established on-demand during the first service request.
            services.AddSingleton<IConnectionMultiplexer>(x =>
                ConnectionMultiplexer.Connect("localhost:6379"));

            services.AddScoped<RedisCacheService>();

            return services;
        }
    }
}
