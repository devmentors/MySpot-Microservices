namespace Micro.Messaging.AzureServiceBus.Internals;

public interface IBrokerConventions
{
    string GetTopicNamingConvention(Type type);
    string GetSubscriptionNamingConvention(Type type, string? subscriberId);
}