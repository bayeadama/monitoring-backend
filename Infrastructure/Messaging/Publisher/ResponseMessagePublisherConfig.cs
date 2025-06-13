namespace Infrastructure.Messaging.Publisher;

public class ResponseMessagePublisherConfig : IResponseMessagePublisherConfig
{
    public string ResponseExchange => "commander.response.exchange";
}