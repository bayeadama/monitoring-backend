namespace Infrastructure.Messaging.Publisher;

public class ResponseMessagePublisherConfig : IResponseMessagePublisherConfig
{
    public string ResponseExchange
    {
        get
        {
            return "commander.response.exchange";
        }
    }
}