using System.Collections.Concurrent;
using EasyNetQ;
using Humanizer;
using Micro.Contexts.Accessors;
using Micro.Messaging.Clients;
using Microsoft.Extensions.Logging;
using IMessage = Micro.Abstractions.IMessage;

namespace Micro.Messaging.RabbitMQ;

internal sealed class RabbitMqBrokerClient : IMessageBrokerClient
{
    private readonly ConcurrentDictionary<Type, string> _names = new();
    private readonly IBus _bus;
    private readonly IMessageContextAccessor _messageContextAccessor;
    private readonly ILogger<RabbitMqBrokerClient> _logger;

    public RabbitMqBrokerClient(IBus bus, IMessageContextAccessor messageContextAccessor,
        ILogger<RabbitMqBrokerClient> logger)
    {
        _bus = bus;
        _messageContextAccessor = messageContextAccessor;
        _logger = logger;
    }

    public async Task SendAsync<T>(MessageEnvelope<T> messageEnvelope, CancellationToken cancellationToken = default)
        where T : IMessage
    {
        var messageContext = messageEnvelope.Context;
        _messageContextAccessor.MessageContext = messageContext;
        var messageName = _names.GetOrAdd(typeof(T), typeof(T).Name.Underscore());
        _logger.LogInformation("Sending a message: {MessageName}  [ID: {MessageId}, Correlation ID: {CorrelationId}]...",
            messageName, messageContext.MessageId, messageContext.Context.CorrelationId);
        await _bus.PubSub.PublishAsync(messageEnvelope.Message, cancellationToken);
    }
}