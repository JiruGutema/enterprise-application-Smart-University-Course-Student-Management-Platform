using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartUniversity.Shared.Kernel.Interface;

namespace SmartUniversity.Shared.Kernel.Infrastructure.Messaging;

public sealed class RabbitMqEventBus : IEventBus
{
    private const string ExchangeName = "smartuniversity.events";

    private readonly RabbitMqConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMqEventBus(RabbitMqConnection connection, IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;
    }

    public async Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : class
    {
        var connection = await _connection.GetConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true
        );

        var routingKey = typeof(TEvent).Name;
        var body = EventSerializer.Serialize(@event);

        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
        };

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body
        );
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler)
        where TEvent : class
    {
        Task.Run(async () =>
        {
            var connection = await _connection.GetConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true
            );

            var queueName = $"{typeof(TEvent).Name}.queue";

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: typeof(TEvent).Name
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var message = EventSerializer.Deserialize<TEvent>(ea.Body.ToArray());

                    using var scope = _scopeFactory.CreateScope();
                    await handler(message);

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    // TODO  dead-letter / retry later
                    await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);
        });
    }
}
