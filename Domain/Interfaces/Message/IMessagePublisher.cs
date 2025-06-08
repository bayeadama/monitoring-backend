namespace Domain.Interfaces.Message;

public interface IMessagePublisher<in TMessage> where TMessage : class
{
    /// <summary>
    /// Sends a message to a given address
    /// </summary>
    /// <param name="message">message to publish</param>
    /// <param name="routingKey">routing's key or destination</param>
    /// <returns></returns>
    Task PublishAsync(TMessage message, string routingKey);
}