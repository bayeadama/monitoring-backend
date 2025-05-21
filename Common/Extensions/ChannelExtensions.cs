using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common;

public static class ChannelExtensions
{
    public static async Task PublishAsync(this IChannel channel , string message, string exchangeName = "", string routingKey = null)
    {
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(exchange: exchangeName, 
            routingKey: routingKey, 
            mandatory: true,
            basicProperties: properties, 
            body: body);
    }
    
    public static async Task AddConsumerAsync(this IChannel channel ,string queueName, 
        AsyncEventHandler<BasicDeliverEventArgs> handler)
    {
       
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            handler?.Invoke(null, ea);
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer);

    }

    public static async Task<QueueDeclareOk> SetupQueueAsync(this IChannel channel, QueueSetup setup)
    {
        if(channel == null)
            throw new ArgumentNullException(nameof(channel));
        
        if (setup == null)
            throw new ArgumentNullException(nameof(setup));
        
        var queueDeclareOk = await channel.QueueDeclareAsync(
            queue:setup.QueueName ?? string.Empty, 
            arguments: setup.Arguments,
            exclusive: false);
        var queueName = queueDeclareOk.QueueName;

        if (setup.RoutingKeys?.Count > 0)
        {
            foreach(var key in setup.RoutingKeys)
                await channel.QueueBindAsync(queueName, exchange:setup.ExchangeName, routingKey: key);
        }
        else
            await channel.QueueBindAsync(queueName, exchange:setup.ExchangeName, routingKey: "");

        
        return queueDeclareOk;
    }
    
}