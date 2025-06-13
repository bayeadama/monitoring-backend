using Application.Services.Agent;
using Application.Services.Commander;
using Application.Services.Listener;
using Domain;
using Domain.Model.ValueObjects;

namespace Presentation.Commander;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ICommanderApplicationService _commanderApplicationService;
    private const string WHO_AM_I_COMMAND = "1";
    private const string MONITORING_COMMAND = "2";

    public Worker(ILogger<Worker> logger, ICommanderApplicationService commanderApplicationService)
    {
        _logger = logger;
        _commanderApplicationService = commanderApplicationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting commander worker");
        Domain.Model.Commander commander = await _commanderApplicationService.InitializeCommanderAsync("Commander1");
        _logger.LogInformation("Commander initialized: {CommanderName}", commander.Name);
        await _commanderApplicationService.RegisterResponseHandlerAsync(commander, (listener, response) =>
        {
            if (listener == null) throw new ArgumentNullException(nameof(listener));
            if (response == null) throw new ArgumentNullException(nameof(response));

            _logger.LogInformation("Response received from listener {ListenerName}: {ResponsePayload}", 
                listener.Name, response.Payload);
        });
        
        
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Choose a command:");
            Console.WriteLine("1. Who Am I");
            Console.WriteLine("2. Monitoring");
            Console.WriteLine("Choose an option:");
            string? chosenValue = Console.ReadLine();
            
            await ProcessCommand(chosenValue, commander);
            
        }
    }
    
    private async Task ProcessCommand(string? command, Domain.Model.Commander commander)
    {
        if(string.IsNullOrEmpty(command))
            return;

        switch (command)
        {
            case WHO_AM_I_COMMAND:
                await ProcessWhoAmICommand(commander);
                break;
            case MONITORING_COMMAND:
                await ProcessMonitoringCommand(commander);
                break;
        }
    }
    
    private async Task ProcessMonitoringCommand(Domain.Model.Commander commander)
    {
        Command monitoringCommand = CommandFactory.CreateMonitoringCommand();
        await _commanderApplicationService.PublishCommandAsync(commander, monitoringCommand, "all");
    }

    private async Task ProcessWhoAmICommand(Domain.Model.Commander commander)
    {
        Command whoAmICommand = CommandFactory.CreateWhoAmiCommand();
        await _commanderApplicationService.PublishCommandAsync(commander, whoAmICommand, "all");
    }
    
    private static async Task RunInfinitely(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}