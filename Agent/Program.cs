using Agent;
using Common;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChannelFactory, ChannelFactoryImpl>();
builder.Services.AddSingleton<IAgentFactory, AgentFactoryImpl>();
builder.Services.AddSingleton<IQueueSetupFactory, QueueSetupFactoryImpl>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();