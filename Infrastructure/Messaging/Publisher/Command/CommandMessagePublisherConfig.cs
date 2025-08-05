namespace Infrastructure.Messaging.Publisher;

public class CommandMessagePublisherConfig : ICommandMessagePublisherConfig
{
    public string CommandExchange => "commander.main.exchange";
}