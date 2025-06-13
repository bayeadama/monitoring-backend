namespace Infrastructure.Messaging.Publisher;

public class CommandMessagePublisherConfig : ICommandMessagePublisherConfig
{
    public string CommandExchange => "commander.pbp.exchange";
}