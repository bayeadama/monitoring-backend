using Commander;
using Common;
using Common.Agent;
using Common.Commander;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChannelFactory, ChannelFactoryImpl>();
builder.Services.AddSingleton<IAgentFactory, AgentFactoryImpl>();
builder.Services.AddSingleton<IQueueSetupFactory, QueueSetupFactoryImpl>();
builder.Services.AddSingleton<ICommanderFactory, CommanderFactoryImpl>();
builder.Services.AddHostedService<CommanderWorker>();

var host = builder.Build();
host.Run();