using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderSystem.Application;
using System;
using System.Collections.Generic;
using System.Text;

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

            return services;
        }
    }
}
