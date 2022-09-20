using System.Diagnostics;

namespace Micro.Contexts;

public sealed class Context : IContext
{
    public string ActivityId { get; }
    public string TraceId { get; }
    public string CorrelationId { get; }
    public string? MessageId { get; }
    public string? CausationId { get; }
    public string? UserId { get; }

    public Context()
    {
        ActivityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
        TraceId = string.Empty;
        CorrelationId = Guid.NewGuid().ToString("N");
    }

    public Context(string activityId, string traceId, string correlationId,
        string? messageId = null, string? causationId = null, string? userId = null)
    {
        ActivityId = activityId;
        TraceId = traceId;
        CorrelationId = correlationId;
        MessageId = messageId;
        CausationId = causationId;
        UserId = userId;
    }
}