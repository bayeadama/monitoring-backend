using Domain.Interfaces.Message;
using Domain.Model.ValueObjects;

namespace Domain.Service;

public class CommanderDomainService
{
    private readonly IMessagePublisher<Command> _commandMessagePublisher;

    public CommanderDomainService(IMessagePublisher<Command> commandMessagePublisher)
    {
        _commandMessagePublisher = commandMessagePublisher ?? throw new ArgumentNullException(nameof(commandMessagePublisher));
    }

    public async Task SendCommand(Command command, string routingKey)
    {
        await _commandMessagePublisher.PublishAsync(command, routingKey);
    }
}