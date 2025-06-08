using Domain.Interfaces.Message;
using Domain.Model;
using Domain.Model.ValueObjects;

namespace Domain.Service;

public class AgentDomainService
{
    private readonly IMessagePublisher<Response> _responseMessagePublisher;

    public AgentDomainService(IMessagePublisher<Response> responseMessagePublisher)
    {
        _responseMessagePublisher = responseMessagePublisher ?? throw new ArgumentNullException(nameof(responseMessagePublisher));
    }


    public async Task SendResponseAsync(Agent agent, Response response)
    {
        string? routingKey = agent.Name;
        await _responseMessagePublisher.PublishAsync(response, routingKey);
    }

}