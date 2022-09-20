using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using Humanizer;
using Micro.Abstractions;
using Micro.Messaging.Brokers;

namespace Micro.Observability.Metrics.Decorators;

[Meter(MetricsKey)]
internal sealed class MessageBrokerMetricsDecorator : IMessageBroker
{
    private const string MetricsKey = "message_broker";
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly IMessageBroker _messageBroker;
    private static readonly Meter Meter = new(MetricsKey);
    private static readonly Counter<long> PublishedMessagesCounter = Meter.CreateCounter<long>("published_messages");

    public MessageBrokerMetricsDecorator(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : IMessage
    {
        var name = Names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        var tags = new KeyValuePair<string, object?>[]
        {
            new("message", name)
        };
        
        await _messageBroker.SendAsync(message, cancellationToken);
        PublishedMessagesCounter.Add(1, tags);
    }
}