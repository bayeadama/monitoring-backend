namespace Common;

public interface IQueueSetupFactory
{
    QueueSetup CreateArbitraryQueueSetupBoundToExchange(string exchangeName, string routingKey = "");
    QueueSetup CreateQueueSetupBoundToExchange(string queueName, string exchangeName, string routingKey = "");
    
}