using System.Diagnostics;
using Micro.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Micro.Transactions.Inbox;

internal sealed class InboxCleaner : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IClock _clock;
    private readonly ILogger<InboxCleaner> _logger;
    private readonly TimeSpan _interval;
    private readonly bool _enabled;
    private int _isProcessing;

    public InboxCleaner(IServiceProvider serviceProvider, IOptions<InboxOptions> options, IClock clock,
        ILogger<InboxCleaner> logger)
    {
        _serviceProvider = serviceProvider;
        _clock = clock;
        _logger = logger;
        _enabled = options.Value.Enabled;
        _interval = options.Value.CleanupInterval ?? TimeSpan.FromHours(1);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) == 1)
            {
                await Task.Delay(_interval, stoppingToken);
                continue;
            }

            _logger.LogInformation("Started cleaning up inbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await using (var scope = _serviceProvider.CreateAsyncScope())
            {
                try
                {
                    var inbox = scope.ServiceProvider.GetRequiredService<IInbox>();
                    await inbox.CleanupAsync(_clock.Current().Subtract(_interval), stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError("There was an error when processing inbox.");
                    _logger.LogError(exception, exception.Message);
                }
                finally
                {
                    Interlocked.Exchange(ref _isProcessing, 0);
                    stopwatch.Stop();
                    _logger.LogInformation($"Finished cleaning up inbox messages in {stopwatch.ElapsedMilliseconds} ms.");
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}