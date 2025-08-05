using Application.Interfaces;
using Domain.Model;
using Domain.Model.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Publisher;
using Infrastructure.Messaging.Receiver;
using Infrastructure.Messaging.Receiver.Command;
using Infrastructure.Messaging.Receiver.Response;
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
        services.AddTransient<IQueueSetupFactory, QueueSetupFactory>();
        services.AddTransient<IResponseMessagePublisherConfig, ResponseMessagePublisherConfig>();
        services.AddTransient<ICommandMessageReceiverConfig, CommandMessageReceiverConfig>();
        services.AddTransient<IResponseMessageReceiverConfig, ResponseMessageReceiverConfig>();
        services.AddTransient<ICommandMessagePublisherConfig, CommandMessagePublisherConfig>();
        services.AddTransient<IMessagePublisher<Agent, Response>, ResponseMessagePublisher>();
        services.AddTransient<IMessageReceiver<Agent, Command>, CommandMessageReceiver>();
        services.AddTransient<IMessageReceiver<Listener, Response>, ResponseMessageReceiver>();
        services.AddTransient<IMessagePublisher<Commander, Command>, CommandMessagePublisher>();
    }
}