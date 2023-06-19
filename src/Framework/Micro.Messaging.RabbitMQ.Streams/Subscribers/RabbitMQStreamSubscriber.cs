using System.Buffers;
using System.Collections.Concurrent;
using Micro.Abstractions;
using Micro.Messaging.Streams;
using Micro.Messaging.Streams.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

namespace Micro.Messaging.RabbitMQ.Streams.Subscribers;

internal sealed class RabbitMQStreamSubscriber : IStreamSubscriber
{
    private readonly ConcurrentDictionary<string, Consumer> _consumers = new();
    private readonly RabbitStreamManager _streamManager;
    private readonly IStreamSerializer _serializer;
    private readonly ILogger<RabbitMQStreamSubscriber> _logger;
    private readonly bool _enabled;
    private readonly RabbitMQStreamsOptions.ConsumerOptions _consumerOptions;

    public RabbitMQStreamSubscriber(RabbitStreamManager streamManager, IStreamSerializer serializer,
        IOptions<RabbitMQStreamsOptions> options, ILogger<RabbitMQStreamSubscriber> logger)
    {
        _streamManager = streamManager;
        _serializer = serializer;
        _logger = logger;
        _enabled = options.Value.Consumer?.Enabled ?? false;
        _consumerOptions = options.Value.Consumer ?? new RabbitMQStreamsOptions.ConsumerOptions();
        if (string.IsNullOrWhiteSpace(_consumerOptions.OffsetType))
        {
            _consumerOptions.OffsetType = "next";
        }
    }

    public async Task SubscribeAsync<T>(string stream, Func<T, Task> handler,
        CancellationToken cancellationToken = default) where T : class, IMessage
    {
        if (!_enabled)
        {
            _logger.LogWarning($"RabbitMQ Streams consumer is disabled, stream: '{stream}' will not be subscribed.");
            return;
        }

        if (_consumers.ContainsKey(stream))
        {
            return;
        }

        var lastOffset = await _streamManager.GetLastOffset(stream);
        IOffsetType offsetType = _consumerOptions.OffsetType.ToLowerInvariant() switch
        {
            "first" => new OffsetTypeFirst(),
            "last" => new OffsetTypeLast(),
            "next" => new OffsetTypeNext(),
            "offset" => new OffsetTypeOffset(lastOffset),
            _ => throw new InvalidOperationException($"Unsupported offset type: '{_consumerOptions.OffsetType}'.")
        };

        var consumer = await _streamManager.CreateConsumerAsync(stream, async (message, ctx) =>
        {
            var bytes = message.Data.Contents.ToArray();
            var payload = _serializer.Deserialize<T>(bytes);
            if (payload is null)
            {
                _logger.LogWarning($"Received a null payload for message with offset: {ctx.Offset}.");
                return;
            }

            await handler(payload);
        }, offsetType, _consumerOptions.OffsetStorageThreshold);
        _consumers.TryAdd(stream, consumer);
    }
}