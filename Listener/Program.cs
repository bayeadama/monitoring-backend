using Common;
using Common.Agent;
using Common.Commander;
using Common.Listener;
using Common.Listener.Config;
using Common.Listener.Impl;
using Listener;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IChannelFactory, ChannelFactoryImpl>();
builder.Services.AddSingleton<IAgentFactory, AgentFactoryImpl>();
builder.Services.AddSingleton<IQueueSetupFactory, QueueSetupFactoryImpl>();
builder.Services.AddSingleton<IListenerFactory, ListenerFactoryImpl>();
builder.Services.AddSingleton<IListenerConfigProvider, DefaultListenerConfigProvider>();
builder.Services.AddHostedService<ListenerWorker>();

var host = builder.Build();
host.Run();