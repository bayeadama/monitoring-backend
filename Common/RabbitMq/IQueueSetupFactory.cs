namespace Common;

public interface IQueueSetupFactory
{
    QueueSetup CreateArbitraryQueueSetupBoundToExchange(
        string exchangeName, 
        List<string> routingKeys = null);
    QueueSetup CreateQueueSetupBoundToExchange(
        string queueName, 
        string exchangeName, 
        List<string> routingKeys = null);
    
}