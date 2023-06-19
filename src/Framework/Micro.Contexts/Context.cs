using System.Diagnostics;

namespace Micro.Contexts;

public sealed class Context : IContext
{
    public string ActivityId { get; }
    public string? UserId { get; }
    public string? MessageId { get; }

    public Context()
    {
        ActivityId = Activity.Current?.Id ?? ActivityTraceId.CreateRandom().ToString();
    }

    public Context(string activityId, string? userId = default, string? messageId = default)
    {
        ActivityId = activityId;
        UserId = userId;
        MessageId = messageId;
    }
}