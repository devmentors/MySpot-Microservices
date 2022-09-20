using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Humanizer;
using Micro.Contexts.Accessors;
using Micro.Messaging.AzureServiceBus.Internals;
using Micro.Messaging.Clients;
using Micro.Messaging.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Messaging.AzureServiceBus;

public static class Extensions
{
    public static IServiceCollection AddAzureServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("azureServiceBUs");
        var options = section.BindOptions<AzureServiceBusOptions>();
        services.Configure<AzureServiceBusOptions>(section);
        
        if (!options.Enabled)
        {
            return services;
        }
        
        var client = new ServiceBusClient(options.ConnectionString);
        var adminClient = new ServiceBusAdministrationClient(options.ConnectionString);
        var contextAccessor = new ContextAccessor();
        var messageContextAccessor = new MessageContextAccessor();
        
        services.AddSingleton(client);
        services.AddSingleton(adminClient);
        services.AddSingleton<IBrokerConventions, AzureServiceBusBrokerConventions>();
        services.AddSingleton<IMessageBrokerClient, AzureServiceBusBrokerClient>();
        services.AddSingleton<IMessageSubscriber, AzureServiceBusMessageSubscriber>();
        services.AddSingleton<IContextAccessor>(contextAccessor);
        services.AddSingleton<IMessageContextAccessor>(messageContextAccessor);
            
        if (options.InitializeResources)
        {
            services.AddHostedService<AzureResourceInitializer>();
        }

        return services;
    }

    internal static string ToMessageKey(this string messageType) => messageType.Underscore();
}