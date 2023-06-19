using System.Reflection;
using Azure.Messaging.ServiceBus.Administration;
using Micro.Abstractions;
using Micro.Attributes;
using Microsoft.Extensions.Hosting;

namespace Micro.Messaging.Azure.ServiceBus.Internals;

internal sealed class AzureResourceInitializer : IHostedService
{
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly IBrokerConventions _conventions;

    public AzureResourceInitializer(ServiceBusAdministrationClient adminClient, IBrokerConventions conventions)
    {
        _adminClient = adminClient;
        _conventions = conventions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var messageTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && typeof(IMessage).IsAssignableFrom(x));

        foreach (var messageType in messageTypes)
        {
            var attribute = messageType.GetCustomAttribute<MessageAttribute>();

            await CreateTopicAsync(messageType, cancellationToken);
            
            if (!string.IsNullOrWhiteSpace(attribute?.Queue))
            {
                await CreateSubscriptionAsync(messageType, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private async Task CreateTopicAsync(Type messageType, CancellationToken cancellationToken)
    {
        var topicName = _conventions.GetTopicNamingConvention(messageType);
        var topicExists = await _adminClient.TopicExistsAsync(topicName, cancellationToken);

        if (!topicExists.Value)
        {
            await _adminClient.CreateTopicAsync(topicName, cancellationToken);
        }
    }
    
    private async Task CreateSubscriptionAsync(Type messageType, CancellationToken cancellationToken)
    {
        var topicName = _conventions.GetTopicNamingConvention(messageType);
        var subscriptionName = _conventions.GetSubscriptionNamingConvention(messageType,null);
        var subscriptionExists = await _adminClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken);

        if (!subscriptionExists.Value)
        {
            await _adminClient.CreateSubscriptionAsync(topicName, subscriptionName, cancellationToken);
        }
    }
}