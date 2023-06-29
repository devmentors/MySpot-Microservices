using System.Reflection;
using Azure.Messaging.ServiceBus.Administration;
using Humanizer;
using Micro.Abstractions;
using Micro.Attributes;
using Microsoft.Extensions.Hosting;

namespace Micro.Messaging.Azure.ServiceBus.Internals;

internal sealed class AzureServiceBusResourceInitializer : IHostedService
{
    private readonly ServiceBusAdministrationClient _adminClient;
    private readonly IBrokerConventions _conventions;

    public AzureServiceBusResourceInitializer(ServiceBusAdministrationClient adminClient, IBrokerConventions conventions)
    {
        _adminClient = adminClient;
        _conventions = conventions;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var messageTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x is {IsClass: true, IsAbstract: false} && typeof(IMessage).IsAssignableFrom(x));

        foreach (var messageType in messageTypes)
        {
            var attribute = messageType.GetCustomAttribute<MessageAttribute>();
            if (attribute is null)
            {
                continue;
            }
            
            await CreateTopicAsync(messageType, cancellationToken);
            if (!string.IsNullOrWhiteSpace(attribute.Queue))
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
            var topicOptions = new CreateTopicOptions(topicName);
            await _adminClient.CreateTopicAsync(topicOptions, cancellationToken);
        }
    }
    
    private async Task CreateSubscriptionAsync(Type messageType, CancellationToken cancellationToken)
    {
        var topicName = _conventions.GetTopicNamingConvention(messageType);
        var subscriptionName = _conventions.GetSubscriptionNamingConvention(messageType,null);
        var subscriptionExists = await _adminClient.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken);
        if (!subscriptionExists.Value)
        {
            var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);
            var ruleOptions = new CreateRuleOptions
            {
                Filter = new CorrelationRuleFilter
                {
                    ApplicationProperties =
                    {
                        ["type"] = messageType.Name.Underscore()
                    }
                }
            };
            await _adminClient.CreateSubscriptionAsync(subscriptionOptions, ruleOptions, cancellationToken);
        }
    }
}