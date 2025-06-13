namespace Application.Interfaces;

public interface IMessagePublisher<TPublisher, TMessage> where TPublisher : class
    where TMessage : class
{
    /// <summary>
    /// Sends a message to a given address
    /// </summary>
    /// <param name="publisher">Message's publisher</param>
    /// <param name="message">message to publish</param>
    /// <param name="destination"></param>
    /// <returns></returns>
    Task PublishAsync(TPublisher publisher, TMessage message, string destination = null);
}