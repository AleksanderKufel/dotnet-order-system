using OrderSystem.Infrastructure;
using OrderSystem.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OrderCreatedConsumer>();

var host = builder.Build();
host.Run();