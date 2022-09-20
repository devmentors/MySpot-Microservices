using Micro.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.Transactions.Inbox;

internal sealed class EfInbox<T> : IInbox where T : DbContext
{
    private readonly T _dbContext;
    private readonly DbSet<InboxMessage> _inboxMessages;
    private readonly IClock _clock;
    private readonly ILogger<EfInbox<T>> _logger;

    public bool Enabled { get; }

    public EfInbox(T dbContext, IClock clock, IOptions<InboxOptions> options, ILogger<EfInbox<T>> logger)
    {
        _dbContext = dbContext;
        _inboxMessages = dbContext.Set<InboxMessage>();
        _clock = clock;
        _logger = logger;
        Enabled = options.Value.Enabled;
    }

    public async Task HandleAsync(string messageId, string messageName, Func<Task> handler,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Received a message with ID: '{messageId}' to be processed.");
        if (await _inboxMessages.AnyAsync(m => m.Id == messageId, cancellationToken))
        {
            _logger.LogWarning($"Message with ID: '{messageId}' was already processed.");
            return;
        }

        _logger.LogInformation($"Processing a message with ID: '{messageId}'...");
        var inboxMessage = new InboxMessage
        {
            Id = messageId,
            Name = messageName,
            ReceivedAt = _clock.Current()
        };

        await handler();
        inboxMessage.ProcessedAt = _clock.Current();
        await _inboxMessages.AddAsync(inboxMessage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Processed a message with ID: '{messageId}'.");
    }

    public async Task CleanupAsync(DateTime? to = null, CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            _logger.LogWarning("Outbox is disabled, incoming messages won't be cleaned up.");
            return;
        }

        var dateTo = to ?? _clock.Current();
        var inboxMessages = await _inboxMessages.Where(x => x.ReceivedAt <= dateTo).ToListAsync(cancellationToken);
        if (!inboxMessages.Any())
        {
            _logger.LogInformation($"No received messages found in inbox till: {dateTo}.");
            return;
        }

        _logger.LogInformation($"Found {inboxMessages.Count} received messages in inbox till: {dateTo}, cleaning up...");
        _inboxMessages.RemoveRange(inboxMessages);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation($"Removed {inboxMessages.Count} received messages from inbox till: {dateTo}.");
    }
}