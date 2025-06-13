namespace Infrastructure.Messaging.Publisher;

public interface ICommandMessagePublisherConfig
{
    string CommandExchange { get; }
}