using System.Collections.Concurrent;
using Humanizer;
using Micro.Abstractions;
using Micro.Messaging.Brokers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Micro.Messaging.Exceptions;

internal sealed class DefaultMessagingExceptionPolicyHandler : IMessagingExceptionPolicyHandler
{
    private const string CancellationTokenKey = nameof(CancellationToken);
    private const string MessageKey = nameof(IMessage);
    private const string ExceptionKey = nameof(Exception);

    private readonly ConcurrentDictionary<Type, string> _messageNames = new();
    private readonly IMessagingExceptionPolicyResolver _messagingExceptionPolicyResolver;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DefaultMessagingExceptionPolicyHandler> _logger;
    private readonly AsyncPolicy _policy;

    public DefaultMessagingExceptionPolicyHandler(IMessagingExceptionPolicyResolver messagingExceptionPolicyResolver,
        IServiceProvider serviceProvider, IOptions<MessagingOptions> options,
        ILogger<DefaultMessagingExceptionPolicyHandler> logger)
    {
        _messagingExceptionPolicyResolver = messagingExceptionPolicyResolver;
        _serviceProvider = serviceProvider;
        _logger = logger;
        var resiliency = options.Value.Resiliency;
        _policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(resiliency.Retries, retry => resiliency.Exponential
                    ? TimeSpan.FromSeconds(Math.Pow(2, retry))
                    : resiliency.RetryInterval ?? TimeSpan.FromSeconds(2),
                (exception, _, retry, context) =>
                {
                    _logger.LogError($"There was an error when processing a message, retry: {retry}/{3}");
                    _logger.LogError(exception, exception.Message);

                    if (!context.ContainsKey(ExceptionKey))
                    {
                        context.Add(ExceptionKey, exception);
                    }

                    var message = (IMessage) context[MessageKey];
                    var failedMessagePolicy = _messagingExceptionPolicyResolver.Resolve(message, exception);
                    if (failedMessagePolicy is null || failedMessagePolicy.Retry)
                    {
                        return;
                    }

                    _logger.LogWarning("Message handling will not be retried according to the specified policy.");
                    var cancellationTokenSource = (CancellationTokenSource) context[CancellationTokenKey];
                    cancellationTokenSource.Cancel();
                });
    }

    public async Task HandleAsync<T>(T message, Func<Task> handler) where T : IMessage
    {
        var messageName = _messageNames.GetOrAdd(typeof(T), message.GetType().Name.Underscore());
        var cts = new CancellationTokenSource();
        var policyContext = new Context
        {
            {CancellationTokenKey, cts},
            {MessageKey, message}
        };

        var result = await _policy.ExecuteAndCaptureAsync((_, _) => handler(), policyContext, cts.Token);
        if (result.Outcome is OutcomeType.Successful)
        {
            return;
        }

        _logger.LogWarning($"Message: '{messageName}' couldn't be handled.");
        var finalException = (Exception) policyContext[ExceptionKey];
        var failedMessagePolicy = _messagingExceptionPolicyResolver.Resolve(message, finalException);
        if (failedMessagePolicy is null)
        {
            _logger.LogWarning("Moving to dead letter queue...");
            throw finalException;
        }

        if (failedMessagePolicy.Publish is not null)
        {
            _logger.LogWarning("Failed message reply will be published...");
            await using var scope = _serviceProvider.CreateAsyncScope();
            {
                await failedMessagePolicy.Publish(scope.ServiceProvider.GetRequiredService<IMessageBroker>());
            }
        }

        if (failedMessagePolicy.UseDeadLetter)
        {
            _logger.LogWarning("Moving to dead letter queue...");
            throw finalException;
        }

        _logger.LogWarning($"Message: '{messageName}' couldn't be handled and won't be moved to dead letter queue.");
    }
}