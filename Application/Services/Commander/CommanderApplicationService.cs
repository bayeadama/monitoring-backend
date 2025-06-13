using Application.Interfaces;
using Application.Services.Listener;
using Domain.Model.ValueObjects;

namespace Application.Services.Commander;

public class CommanderApplicationService: ListenerApplicationService, ICommanderApplicationService
{
    private readonly IMessageReceiver<Domain.Model.Listener, Response> _responseReceiver;
    private readonly IMessagePublisher<Domain.Model.Commander, Command> _commandMessagePublisher;

    public CommanderApplicationService(IMessageReceiver<Domain.Model.Listener, Response> responseReceiver, IMessagePublisher<Domain.Model.Commander, Command> commandMessagePublisher):
        base(responseReceiver)
    {
        _responseReceiver = responseReceiver;
        _commandMessagePublisher = commandMessagePublisher;
    }


    public Task<Domain.Model.Commander> InitializeCommanderAsync(string commanderName)
    {
        var commander = new Domain.Model.Commander
        {
            Name = commanderName
        };
        return Task.FromResult(commander);
    }

    public async Task RegisterResponseHandlerAsync(Domain.Model.Commander commander,
        Action<Domain.Model.Listener, Response> responseHandler)
    {
        await base.RegisterResponseHandlerAsync(commander, responseHandler);
    }

    public async Task PublishCommandAsync(Domain.Model.Commander commander, Command command, string destination)
    {
        if (commander == null)
            throw new ArgumentNullException(nameof(commander));

        if (command == null)
            throw new ArgumentNullException(nameof(command));
        
        if (string.IsNullOrEmpty(destination))
            throw new ArgumentException("Destination cannot be null or empty.", nameof(destination));

        await _commandMessagePublisher.PublishAsync(commander, command, destination);
    }
}