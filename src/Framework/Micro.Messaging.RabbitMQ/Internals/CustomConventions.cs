using System.Collections.Concurrent;
using System.Reflection;
using EasyNetQ;
using Humanizer;
using Micro.Attributes;

namespace Micro.Messaging.RabbitMQ.Internals;

internal sealed class CustomConventions : IConventions
{
    private readonly ConcurrentDictionary<Type, string> _exchanges = new();
    private readonly ConcurrentDictionary<Type, string> _queues = new();
    private readonly ConcurrentDictionary<Type, string> _queueTypes = new();
    private readonly ConcurrentDictionary<Type, string> _topics = new();
    private readonly ConcurrentDictionary<Type, string> _errorQueues = new();
    private readonly ConcurrentDictionary<string, Type> _typeQueues = new();

    public ExchangeNameConvention ExchangeNamingConvention { get; }
    public TopicNameConvention TopicNamingConvention { get; }
    public QueueNameConvention QueueNamingConvention { get; }
    public QueueTypeConvention QueueTypeConvention { get; }
    public RpcRoutingKeyNamingConvention RpcRoutingKeyNamingConvention { get; }
    public RpcExchangeNameConvention RpcRequestExchangeNamingConvention { get; }
    public RpcExchangeNameConvention RpcResponseExchangeNamingConvention { get; }
    public RpcReturnQueueNamingConvention RpcReturnQueueNamingConvention { get; }
    public ConsumerTagConvention ConsumerTagConvention { get; }
    public ErrorQueueNameConvention ErrorQueueNamingConvention { get; }
    public ErrorExchangeNameConvention ErrorExchangeNamingConvention { get; }

    public CustomConventions()
    {
        ExchangeNamingConvention = type => _exchanges.GetOrAdd(type, _ =>
        {
            var attribute = GetMessageAttribute(type);
            if (!string.IsNullOrWhiteSpace(attribute.Topic))
            {
                return attribute.Topic;
            }

            var exchange = string.IsNullOrWhiteSpace(type.FullName)
                ? GetTypeKey(type)
                : type.FullName.Split(".")[0].Underscore();

            return exchange;
        });

        QueueNamingConvention = (type, subscriberId) => _queues.GetOrAdd(type, _ =>
        {
            var attribute = GetMessageAttribute(type);
            var queue = attribute.Queue;
            if (string.IsNullOrWhiteSpace(queue))
            {
                queue = string.IsNullOrWhiteSpace(type.FullName) ? GetTypeKey(type) : type.FullName.Underscore();
            }

            queue = string.IsNullOrWhiteSpace(subscriberId) ? queue : $"{queue}_{subscriberId}";
            _typeQueues.TryAdd(queue, type);

            return queue;
        });

        TopicNamingConvention = type => _topics.GetOrAdd(type, _ =>
        {
            var attribute = GetMessageAttribute(type);
            var topic = string.IsNullOrWhiteSpace(attribute.Key) ? GetTypeKey(type) : attribute.Key;

            return topic;
        });

        QueueTypeConvention = type => _queueTypes.GetOrAdd(type, _ =>
        {
            var attribute = GetMessageAttribute(type);

            return attribute.QueueType;
        });

        RpcRoutingKeyNamingConvention = _ => string.Empty;

        ErrorQueueNamingConvention = info =>
        {
            const string defaultName = "errored_messages";
            if (!_typeQueues.TryGetValue(info.Queue, out var type))
            {
                return defaultName;
            }

            return _errorQueues.GetOrAdd(type, _ =>
            {
                var attribute = GetMessageAttribute(type);
                var queue = string.IsNullOrWhiteSpace(attribute.ErrorQueue) ? defaultName : attribute.ErrorQueue;

                return queue;
            });
        };

        ErrorExchangeNamingConvention = receivedInfo => "errors_" + receivedInfo.RoutingKey;

        RpcRequestExchangeNamingConvention = _ => "rpc_requests";

        RpcResponseExchangeNamingConvention = _ => "rpc_responses";

        RpcReturnQueueNamingConvention = _ => "responses." + Guid.NewGuid();

        ConsumerTagConvention = () => Guid.NewGuid().ToString();
    }

    private static string GetTypeKey(MemberInfo type) => type.Name.ToMessageKey();

    private static MessageAttribute GetMessageAttribute(MemberInfo type)
        => type.GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();
}