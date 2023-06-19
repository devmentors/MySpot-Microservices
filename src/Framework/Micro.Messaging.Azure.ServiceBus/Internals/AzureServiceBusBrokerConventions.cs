using System.Collections.Concurrent;
using System.Reflection;
using Humanizer;
using Micro.Attributes;

namespace Micro.Messaging.Azure.ServiceBus.Internals;

internal sealed class AzureServiceBusBrokerConventions : IBrokerConventions
{
    private readonly ConcurrentDictionary<Type, string> _queues = new();
    private readonly ConcurrentDictionary<Type, string> _topics = new();
    private readonly ConcurrentDictionary<string, Type> _typeQueues = new();
    
    public string GetTopicNamingConvention(Type type) 
        => _topics.GetOrAdd(type, _ =>
        {
            var attribute = GetMessageAttribute(type);
            if (!string.IsNullOrWhiteSpace(attribute.Exchange))
            {
                return attribute.Exchange;
            }

            var topic = string.IsNullOrWhiteSpace(type.FullName)
                ? GetTypeKey(type)
                : type.FullName.Split(".")[0].Underscore();

            return topic;
        });
    
    public string GetSubscriptionNamingConvention(Type type, string? subscriberId) 
        => _queues.GetOrAdd(type, _ =>
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

    private static string GetTypeKey(MemberInfo type) => type.Name.ToMessageKey();

    private static MessageAttribute GetMessageAttribute(MemberInfo type)
        => type.GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();
}