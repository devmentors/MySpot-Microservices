using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.Transactions.Outbox;

internal sealed class OutboxSender : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxSender> _logger;
    private readonly TimeSpan _senderInterval;
    private readonly bool _enabled;
    private int _isProcessing;

    public OutboxSender(IServiceProvider serviceProvider, IOptions<OutboxOptions> options, ILogger<OutboxSender> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _enabled = options.Value.Enabled;
        _senderInterval = options.Value.SenderInterval ?? TimeSpan.FromSeconds(5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogWarning("Outbox is disabled");
            return;
        }

        _logger.LogInformation($"Outbox is enabled, sender interval: {_senderInterval}");
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_senderInterval, stoppingToken);
                continue;
            }
                
            _logger.LogInformation("Started processing outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    await outbox.PublishUnsentAsync(stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError("There was an error when processing outbox.");
                    _logger.LogError(exception, exception.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    _logger.LogInformation($"Finished processing outbox messages in {stopwatch.ElapsedMilliseconds} ms.");
                }
            }

            await Task.Delay(_senderInterval, stoppingToken);
        }
    }
}