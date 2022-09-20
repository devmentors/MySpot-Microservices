using System.Collections.Concurrent;
using System.Diagnostics;
using Humanizer;
using Micro.Abstractions;
using Micro.Contexts;
using Micro.Messaging.RabbitMQ.Internals;

namespace Micro.Observability.Tracing.Decorators;

internal sealed class MessageHandlerTracingDecorator : IMessageHandler
{
    public const string ActivitySourceName = "message_handler";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private static readonly ConcurrentDictionary<Type, string> Names = new();
    private readonly IMessageHandler _messageHandler;
    private readonly IContextProvider _contextProvider;

    public MessageHandlerTracingDecorator(IMessageHandler messageHandler, IContextProvider contextProvider)
    {
        _messageHandler = messageHandler;
        _contextProvider = contextProvider;
    }

    public async Task HandleAsync<T>(Func<IServiceProvider, T, CancellationToken, Task> handler, T message,
        CancellationToken cancellationToken = default) where T : IMessage
    {
        var context = _contextProvider.Current();
        var name = Names.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        using var activity = ActivitySource.StartActivity("subscriber", ActivityKind.Consumer, context.ActivityId);
        activity?.SetTag("message", name);
        activity?.SetTag("correlation_id", context.CorrelationId);
        activity?.SetTag("causation_id", context.CausationId);
        
        try
        {
            await _messageHandler.HandleAsync(handler, message, cancellationToken);
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.ToString());
            throw;
        }
    }
}