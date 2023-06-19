using System.Collections.Concurrent;
using System.Reflection;
using Humanizer;
using Micro.Abstractions;
using Micro.Contexts;
using Micro.Messaging;
using Micro.Messaging.Clients;
using Micro.Serialization;
using Micro.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.Transactions.Outbox;

internal sealed class EfOutbox<T> : IOutbox where T : DbContext
{
    private static readonly ConcurrentDictionary<Type, Type> EnvelopeTypes = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> SendMethods = new();
    private static readonly ConcurrentDictionary<Type, string> MessageNames = new();
    private static readonly ConcurrentDictionary<string, Type> MessageTypes = new();

    private readonly List<OutboxMessage> _awaitingMessages = new();
    private readonly T _dbContext;
    private readonly DbSet<OutboxMessage> _set;
    private readonly IClock _clock;
    private readonly IMessageBrokerClient _messageBrokerClient;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<EfOutbox<T>> _logger;
    private readonly MethodInfo _sendMethod;

    public bool Enabled { get; }

    public EfOutbox(T dbContext, IClock clock, IMessageBrokerClient messageBrokerClient, IJsonSerializer jsonSerializer,
        IOptions<OutboxOptions> options,  ILogger<EfOutbox<T>> logger)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<OutboxMessage>();
        _clock = clock;
        _messageBrokerClient = messageBrokerClient;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
        _sendMethod = _messageBrokerClient.GetType().GetMethod(nameof(IMessageBrokerClient.SendAsync)) ??
                      throw new InvalidOperationException("Send method was not defined.");
        Enabled = options.Value.Enabled;
    }

    public async Task SaveAsync<TMessage>(MessageEnvelope<TMessage> message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var outboxMessage = new OutboxMessage
        {
            Id = message.Context.MessageId,
            Context = _jsonSerializer.Serialize(message.Context),
            Name = message.Message.GetType().Name.Underscore(),
            Data = _jsonSerializer.Serialize<object>(message.Message),
            Type = message.Message.GetType().AssemblyQualifiedName ?? string.Empty,
            ReceivedAt = _clock.Current()
        };

        await _set.AddAsync(outboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _awaitingMessages.Add(outboxMessage);
        _logger.LogInformation($"Saved a message: '{outboxMessage.Name}' with ID: '{outboxMessage.Id}' to the outbox.");
    }

    public async Task PublishUnsentAsync(CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be sent.");
            return;
        }
            
        var unsentMessages = await _set.Where(x => x.SentAt == null).ToListAsync(cancellationToken);
        if (!unsentMessages.Any())
        {
            _logger.LogInformation("No unsent messages found in outbox.");
            return;
        }

        _logger.LogInformation($"Found {unsentMessages.Count} unsent messages in outbox, sending...");
        foreach (var outboxMessage in unsentMessages.OrderBy(x => x.ReceivedAt))
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = _clock.Current();
            _dbContext.Entry(outboxMessage).Property(x => x.SentAt).IsModified = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task PublishAwaitingAsync(CancellationToken cancellationToken = default)
    {
        if (!_awaitingMessages.Any())
        {
            return;
        }

        foreach (var outboxMessage in _awaitingMessages)
        {
            await SendMessageAsync(outboxMessage, cancellationToken);
            outboxMessage.SentAt = _clock.Current();
            _set.Attach(outboxMessage);
            _dbContext.Entry(outboxMessage).Property(x => x.SentAt).IsModified = true;
        }

        _awaitingMessages.Clear();
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SendMessageAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        var type = MessageTypes.GetOrAdd(outboxMessage.Type, _ =>
        {
            var type = Type.GetType(outboxMessage.Type);
            if (type is null)
            {
                throw new InvalidOperationException($"Type was not found for: '{outboxMessage.Type}'.");
            }

            return type;
        });

        if (_jsonSerializer.Deserialize(outboxMessage.Data, type) is not IMessage message)
        {
            throw new InvalidOperationException($"Invalid message type in outbox: '{type.Name}'," +
                                                $"name: '{outboxMessage.Name}', ID: '{outboxMessage.Id}'.");
        }

        var messageId = outboxMessage.Id;
        var context = _jsonSerializer.Deserialize<Context>(outboxMessage.Context) ?? new Context();
        var name = MessageNames.GetOrAdd(type, type.Name.Underscore());
        
        var envelopeType = EnvelopeTypes.GetOrAdd(type, typeof(MessageEnvelope<>).MakeGenericType(type));
        var sendMethod = SendMethods.GetOrAdd(type, _sendMethod.MakeGenericMethod(type));
        
        var messageContext =  new MessageContext(messageId, context);
        var envelope = Activator.CreateInstance(envelopeType, message, messageContext);
        
        _logger.LogInformation("Sending a message from outbox: {Name} [Message ID: {MessageId}, Activity ID: {ActivityId}]...",
            name, messageId, context.ActivityId);
        
        var sendMessageTask = sendMethod.Invoke(_messageBrokerClient, new[] {envelope, cancellationToken});
        if (sendMessageTask is null)
        {
            throw new InvalidOperationException("Missing a task when sending a message.");
        }

        await (Task) sendMessageTask;
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _clock.Current();
        var outboxMessages = await _set.Where(x => x.SentAt != null
                                                 && x.ReceivedAt <= dateTo).ToListAsync(cancellationToken);
        if (!outboxMessages.Any())
        {
            _logger.LogInformation($"No sent messages found in outbox till: {dateTo}.");
            return;
        }

        _logger.LogInformation($"Found {outboxMessages.Count} sent messages in outbox till: {dateTo}, cleaning up...");
        _set.RemoveRange(outboxMessages);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Removed {outboxMessages.Count} sent messages from outbox till: {dateTo}.");
    }
}