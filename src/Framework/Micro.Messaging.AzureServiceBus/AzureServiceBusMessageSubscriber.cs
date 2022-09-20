using System.Reflection;
using Azure.Messaging.ServiceBus;
using Micro.Abstractions;
using Micro.Attributes;
using Micro.Handlers;
using Micro.Messaging.AzureServiceBus.Internals;
using Micro.Messaging.Subscribers;
using Micro.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Micro.Messaging.AzureServiceBus;

public class AzureServiceBusMessageSubscriber : IMessageSubscriber
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBrokerConventions _conventions;
    private readonly IJsonSerializer _serializer;
    private readonly ServiceBusClient _client;
    private readonly ILogger<AzureServiceBusMessageSubscriber> _logger;

    public AzureServiceBusMessageSubscriber(IServiceProvider serviceProvider, IBrokerConventions conventions,
        IJsonSerializer serializer, ServiceBusClient client, ILogger<AzureServiceBusMessageSubscriber> logger)
    {
        _serviceProvider = serviceProvider;
        _conventions = conventions;
        _serializer = serializer;
        _client = client;
        _logger = logger;
    }

    public IMessageSubscriber Command<T>() where T : class, ICommand
        => Message<T>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Event<T>() where T : class, IEvent
        => Message<T>((serviceProvider, @event, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().PublishAsync(@event, cancellationToken));

    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler)
        where T : class, IMessage
    {
        var messageAttribute = typeof(T).GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();
        var subscriberId = messageAttribute.SubscriptionId;
        
        var topicName = _conventions.GetTopicNamingConvention(typeof(T));
        var subscriptionName = _conventions.GetSubscriptionNamingConvention(typeof(T), subscriberId);
        var options = new ServiceBusProcessorOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock, // default
        };
        
        var processor = _client.CreateProcessor(topicName, subscriptionName, options);

        processor.ProcessMessageAsync += async (args) =>
        {
            var json = args.Message.Body.ToString();
            var message = _serializer.Deserialize<T>(json);
            if (message is null)
            {
                throw new InvalidOperationException("Couldn't deserialize the message.");
            }
                
            await handler(_serviceProvider, message, args.CancellationToken);
            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        };

        processor.ProcessErrorAsync += async (args) =>
        {
            await Task.CompletedTask;
            var exception = args.Exception;

            if (true) // determine exception type
            {
                _logger.LogError(exception, exception.Message);
            }
        };

        _ = processor.StartProcessingAsync();

        return this;
    }
}