using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using EasyNetQ;
using Micro.Contexts;
using Micro.Contexts.Accessors;

namespace Micro.Messaging.RabbitMQ.Internals;

internal sealed class CustomMessageSerializationStrategy : IMessageSerializationStrategy
{
    private const string ActivityIdKey = "activity-id";
    private const string CausationIdKey = "causation-id";
    private const string TraceIdKey = "trace-id";
    private const string UserIdKey = "user-id";
    
    private readonly ConcurrentDictionary<Type, string> _typeNames = new();
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    private readonly ISerializer _serializer;
    private readonly IMessageContextAccessor _messageContextAccessor;
    private readonly IContextAccessor _contextAccessor;

    public CustomMessageSerializationStrategy(IMessageTypeRegistry messageTypeRegistry, ISerializer serializer,
        IMessageContextAccessor messageContextAccessor, IContextAccessor contextAccessor)
    {
        _messageTypeRegistry = messageTypeRegistry;
        _serializer = serializer;
        _messageContextAccessor = messageContextAccessor;
        _contextAccessor = contextAccessor;
    }

    public SerializedMessage SerializeMessage(IMessage message)
    {
        var messageContext = _messageContextAccessor.MessageContext;
        var messageBody = _serializer.MessageToBytes(message.MessageType, message.GetBody());
        var messageProperties = message.Properties;
        messageProperties.Type = _typeNames.GetOrAdd(message.MessageType, message.MessageType.Name.ToMessageKey());

        if (!string.IsNullOrWhiteSpace(messageContext?.MessageId))
        {
            messageProperties.MessageId = messageContext.MessageId;
        }
        
        if (!string.IsNullOrWhiteSpace(messageContext?.Context.CorrelationId))
        {
            messageProperties.CorrelationId = messageContext.Context.CorrelationId;
        }
        
        if (!string.IsNullOrWhiteSpace(Activity.Current?.Id))
        {
            messageProperties.Headers.TryAdd(ActivityIdKey, Activity.Current.Id);
        }
        
        if (!string.IsNullOrWhiteSpace(messageContext?.Context.TraceId))
        {
            messageProperties.Headers.TryAdd(TraceIdKey, messageContext.Context.TraceId);
        }
        
        // Access the context for the external message (parent) that might be currently being handled
        if (!string.IsNullOrWhiteSpace(_contextAccessor.Context?.CausationId))
        {
            messageProperties.Headers.TryAdd(CausationIdKey, _contextAccessor.Context.CausationId);
        }
        
        if (!string.IsNullOrWhiteSpace(messageContext?.Context.UserId))
        {
            messageProperties.Headers.TryAdd(UserIdKey, messageContext.Context.UserId);
        }

        return new SerializedMessage(messageProperties, messageBody);
    }

    public IMessage DeserializeMessage(MessageProperties properties, in ReadOnlyMemory<byte> body)
    {
        var type = _messageTypeRegistry.Resolve(properties.Type);
        if (type is null)
        {
            throw new Exception($"Message was not registered for type: '{properties.Type}'.");
        }

        var activityId = GetHeaderValue(properties, ActivityIdKey);
        var traceId = GetHeaderValue(properties, TraceIdKey);
        var causationId = GetHeaderValue(properties, CausationIdKey);
        var userId = GetHeaderValue(properties, UserIdKey);

        _contextAccessor.Context = new Context(activityId, traceId, properties.CorrelationId,
            properties.MessageId, causationId, userId);

        var messageBody = _serializer.BytesToMessage(type, body);
        return MessageFactory.CreateInstance(type, messageBody, properties);
    }

    private static string GetHeaderValue(MessageProperties properties, string key)
        => properties.Headers.TryGetValue(key, out var bytes)
            ? Encoding.UTF8.GetString(bytes as byte[] ?? Array.Empty<byte>())
            : string.Empty;
}