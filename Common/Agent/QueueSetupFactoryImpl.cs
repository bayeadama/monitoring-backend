namespace Common;

public class QueueSetupFactoryImpl : IQueueSetupFactory
{

    public QueueSetup CreateArbitraryQueueSetupBoundToExchange(string exchangeName, string routingKey = "")
    {
        return new QueueSetup
        {
            ExchangeName = exchangeName,
            RoutingKey = routingKey
        };
    }

    public QueueSetup CreateQueueSetupBoundToExchange(string queueName, string exchangeName, string routingKey = "")
    {
        return new QueueSetup
        {
            QueueName = queueName,
            ExchangeName = exchangeName,
            RoutingKey = routingKey
        };
    }
}