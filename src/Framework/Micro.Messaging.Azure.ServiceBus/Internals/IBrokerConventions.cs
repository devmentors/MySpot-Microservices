namespace Micro.Messaging.Azure.ServiceBus.Internals;

public interface IBrokerConventions
{
    string GetTopicNamingConvention(Type type);
    string GetSubscriptionNamingConvention(Type type, string? subscriberId);
}