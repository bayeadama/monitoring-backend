using System.Text.Json;
using Common.Commander;
using Common.Models;

namespace Commander;

public class CommanderWorker : BackgroundService
{
    private readonly ILogger<CommanderWorker> _logger;
    private readonly ICommanderFactory  _commanderFactory;
    private readonly ICommanderConfigProvider  _commanderConfigProvider;
    private const string WHO_AM_I_COMMAND = "1";
    private const string MONITORING_COMMAND = "2";


    public CommanderWorker(ILogger<CommanderWorker> logger, ICommanderFactory commanderFactory, ICommanderConfigProvider commanderConfigProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commanderFactory = commanderFactory ?? throw new ArgumentNullException(nameof(commanderFactory));
        _commanderConfigProvider = commanderConfigProvider ?? throw new ArgumentNullException(nameof(commanderConfigProvider));
        ;
    }

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting commander");
        CommanderConfig config = _commanderConfigProvider.GetConfig();
        ICommander commander = await _commanderFactory.Create(config);
        commander.OnResponseReceived += CommanderOnOnResponseReceived;
        
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

    private void CommanderOnOnResponseReceived(Response response)
    {
        string json = JsonSerializer.Serialize(response);
        _logger.LogInformation($"Received response (in commander): {json}");
        
        switch (response.FromCommand.Name)
        {
            case CommandName.WhoAmI:
                _logger.LogInformation("WhoAmI");
                ProcessWhoAmIResponse(response);
                break;
            case CommandName.Monitoring:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ProcessWhoAmIResponse(Response response)
    {
        StandardWhoAmIResponse? whoAmIResponse = 
            JsonSerializer.Deserialize<StandardWhoAmIResponse>(response.Payload, JsonSerializerOptions.Web);
        _logger.LogInformation($"Received response from: {whoAmIResponse.AgentId}");
    }

    private async Task ProcessCommand(string? command, ICommander commander)
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

    private async Task ProcessMonitoringCommand(ICommander commander)
    {
        Command monitoringCommand = CommandFactory.CreateMonitoringCommand();
        await commander.SendCommandAsync(monitoringCommand, "all");
    }

    private async Task ProcessWhoAmICommand(ICommander commander)
    {
        Command whoAmICommand = CommandFactory.CreateWhoAmiCommand();
        await commander.SendCommandAsync(whoAmICommand, "all");
    }
    
}