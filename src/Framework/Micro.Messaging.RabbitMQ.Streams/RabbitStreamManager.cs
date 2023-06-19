using System.Net;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace Micro.Messaging.RabbitMQ.Streams;

public sealed class RabbitStreamManager
{
    private readonly IOptions<RabbitMQStreamsOptions> _options;
    private readonly ILogger<RabbitStreamManager> _logger;
    private StreamSystem _system = null!;
    
    public string ClientName { get; }
    public string ConsumerReference { get; }
    public string ProducerReference { get; }

    public RabbitStreamManager(IOptions<RabbitMQStreamsOptions> options, ILogger<RabbitStreamManager> logger)
    {
        _options = options;
        _logger = logger;
        ClientName = GetName(_options.Value.ClientName) ?? $"client-{Guid.NewGuid()}";
        ConsumerReference = GetName(_options.Value.Consumer?.Reference) ?? $"{ClientName}.consumer";
        ProducerReference = GetName(_options.Value.Producer?.Reference) ?? $"{ClientName}.producer";
    }

    public async Task InitAsync()
    {
        var host = _options.Value.Server.Split(":");
        var ipAddress = IPAddress.Parse(host[0]);
        var port = int.Parse(host[1]);

        _system = await StreamSystem.Create(new StreamSystemConfig
        {
            UserName = _options.Value.Username,
            Password = _options.Value.Password,
            VirtualHost = _options.Value.VirtualHost,
            ClientProvidedName = ClientName,
            AddressResolver = new AddressResolver(new IPEndPoint(ipAddress, port))
        });

        await InitStreamsAsync(_options.Value.Consumer?.Streams);
        await InitStreamsAsync(_options.Value.Producer?.Streams);
    }
    
    public Task<ulong> GetLastPublishingId(string stream) => _system.QuerySequence(ProducerReference, stream);

    public Task<ulong> GetLastOffset(string stream) => _system.QueryOffset(ConsumerReference, stream);

    private async Task InitStreamsAsync(IEnumerable<RabbitMQStreamsOptions.StreamOptions>? streams)
    {
        foreach (var stream in streams ?? Enumerable.Empty<RabbitMQStreamsOptions.StreamOptions>())
        {
            if (!string.IsNullOrWhiteSpace(stream.Name))
            {
                await CreateStreamAsync(stream.Name, stream.MaxAge, stream.MaxLengthBytes, stream.MaxSegmentSizeBytes);
            }
        }
    }

    public async Task CreateStreamAsync(string stream, TimeSpan? maxAge = default,
        ulong? maxLengthBytes = default, int? maxSegmentSizeBytes = default)
    {
        if (await _system.StreamExists(stream))
        {
            return;
        }

        var streamSpec = new StreamSpec(stream);
        if (maxAge.HasValue)
        {
            streamSpec.MaxAge = maxAge.Value;
        }

        if (maxLengthBytes.HasValue)
        {
            streamSpec.MaxLengthBytes = maxLengthBytes.Value;
        }

        if (maxSegmentSizeBytes.HasValue)
        {
            streamSpec.MaxSegmentSizeBytes = maxSegmentSizeBytes.Value;
        }

        await _system.CreateStream(streamSpec);
    }

    public async Task<Consumer> CreateConsumerAsync(string stream, Func<Message, MessageContext, Task> handler,
        IOffsetType? offsetType = default, ulong? offsetStorageThreshold = default)
    {
        var offsetStorageEnabled = offsetStorageThreshold is > 0;
        var consumer = await Consumer.Create(new ConsumerConfig(_system, stream)
        {
            Reference = ConsumerReference,
            ClientProvidedName = ClientName,
            OffsetSpec = offsetType ?? new OffsetTypeNext(),
            MessageHandler = async (_, rawConsumer, ctx, message) =>
            {
                try
                {
                    await handler(message, ctx);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }

                if (offsetStorageEnabled && ctx.Offset % offsetStorageThreshold is 0)
                {
                    await rawConsumer.StoreOffset(ctx.Offset);
                }
            }
        });

        return consumer;
    }

    public async Task<Producer> CreateProducerAsync(string stream)
    {
        var producer = await Producer.Create(new ProducerConfig(_system, stream)
        {
            ClientProvidedName = ProducerReference,
            ConfirmationHandler = confirmation =>
            {
                if (confirmation.Status is not ConfirmationStatus.Confirmed)
                {
                    _logger.LogError($"Message with publishing ID: {confirmation.PublishingId} has failed with status: {confirmation.Status}.");
                }
                
                return Task.CompletedTask;
            }
        });

        return producer;
    }

    private static string? GetName(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var name = Assembly.GetEntryAssembly()?.FullName?.Split(",").First();
        return string.IsNullOrWhiteSpace(name) ? default : name;
    }
}