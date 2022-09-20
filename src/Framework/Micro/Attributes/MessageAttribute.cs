namespace Micro.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class MessageAttribute : Attribute
{
    public string Exchange { get; }
    public string Topic { get; }
    public string Queue { get; }
    public string QueueType { get; }
    public string ErrorQueue { get; }
    public string SubscriptionId { get; }

    public MessageAttribute(string? exchange = null, string? topic = null, string? queue = null,
        string? queueType = null, string? errorQueue = null, string? subscriptionId = null)
    {
        Exchange = exchange ?? string.Empty;
        Topic = topic ?? string.Empty;
        Queue = queue ?? string.Empty;
        QueueType = queueType ?? string.Empty;
        ErrorQueue = errorQueue ?? string.Empty;
        SubscriptionId = subscriptionId ?? string.Empty;
    }
}