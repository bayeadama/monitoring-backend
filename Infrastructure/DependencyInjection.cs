using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Publisher;
using Infrastructure.Messaging.Receiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IChannel>(serviceProvider =>
        {
            var cfg = serviceProvider.GetService<IConfiguration>();
            var serviceUri = cfg["RabbitMQ:Uri"];
            if (serviceUri == null)
                return null;

            var uri = new Uri(serviceUri);
            return uri.CreateChannelAsync().GetAwaiter().GetResult();
        });
        services.AddSingleton<IQueueSetupFactory, QueueSetupFactory>();
        services.AddSingleton<IResponseMessagePublisherConfig, ResponseMessagePublisherConfig>();
        services.AddSingleton<ICommandMessageReceiverConfig, CommandMessageReceiverConfig>();
        services.AddSingleton<IResponseMessageReceiverConfig, ResponseMessageReceiverConfig>();
        services.AddSingleton<ICommandMessagePublisherConfig, CommandMessagePublisherConfig>();
        services.AddSingleton<IMessagePublisher<Agent, Response>, ResponseMessagePublisher>();
        services.AddSingleton<IMessageReceiver<Agent, Command>, CommandMessageReceiver>();
        services.AddSingleton<IMessageReceiver<Listener, Response>, ResponseMessageReceiver>();
        services.AddSingleton<IMessagePublisher<Commander, Command>, CommandMessagePublisher>();
    }
}