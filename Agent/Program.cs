using Agent;
using Common;
using Common.Agent;
using Common.Commander;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChannelFactory, ChannelFactoryImpl>();
builder.Services.AddSingleton<IAgentFactory, AgentFactoryImpl>();
builder.Services.AddSingleton<IQueueSetupFactory, QueueSetupFactoryImpl>();
builder.Services.AddSingleton<IAgentConfigProvider, DefaultAgentConfigProvider>();
builder.Services.AddHostedService<AgentWorker>();

var host = builder.Build();
host.Run();