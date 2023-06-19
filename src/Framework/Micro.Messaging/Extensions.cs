using Micro.Messaging.Brokers;
using Micro.Messaging.Clients;
using Micro.Messaging.Exceptions;
using Micro.Messaging.Streams;
using Micro.Messaging.Streams.Serialization;
using Micro.Messaging.Subscribers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Messaging;

public static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("messaging");
        services.Configure<MessagingOptions>(section);
        services.AddTransient<IMessageBroker, MessageBroker>();
        services.AddSingleton<IMessageBrokerClient, DefaultMessageBrokerClient>();
        services.AddSingleton<IMessageSubscriber, DefaultMessageSubscriber>();
        services.AddSingleton<IMessagingExceptionPolicyResolver, DefaultMessagingExceptionPolicyResolver>();
        services.AddSingleton<IMessagingExceptionPolicyHandler, DefaultMessagingExceptionPolicyHandler>();
        services.AddSingleton<IStreamSerializer, SystemTextJsonStreamSerializer>();
        services.AddSingleton<IStreamPublisher, DefaultStreamPublisher>();
        services.AddSingleton<IStreamSubscriber, DefaultStreamSubscriber>();
        
        return services;
    }
    
    public static IMessageSubscriber Subscribe(this IApplicationBuilder app)
        => app.ApplicationServices.GetRequiredService<IMessageSubscriber>();
}