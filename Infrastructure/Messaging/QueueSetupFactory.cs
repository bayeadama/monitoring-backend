using Infrastructure.Config;

namespace Infrastructure.Messaging;

public class QueueSetupFactory : IQueueSetupFactory
{
    public QueueSetup CreateArbitraryQueueSetupBoundToExchange(string exchangeName, List<string> routingKeys = null)
    {
        return new QueueSetup
        {
            ExchangeName = exchangeName,
            RoutingKeys = routingKeys
        };
    }

    public QueueSetup CreateQueueSetupBoundToExchange(string queueName, string exchangeName,
        List<string> routingKeys = null)
    {
        return new QueueSetup
        {
            QueueName = queueName,
            ExchangeName = exchangeName,
            RoutingKeys = routingKeys
        };
    }
}