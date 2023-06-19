namespace Micro.Messaging.RabbitMQ.Streams;

public sealed class RabbitMQStreamsOptions
{
    public bool Enabled { get; set; }
    public string Server { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public ConsumerOptions? Consumer { get; set; }
    public ProducerOptions? Producer { get; set; }

    public class ConsumerOptions
    {
        public bool Enabled { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string OffsetType { get; set; } = string.Empty;
        public ulong? OffsetStorageThreshold { get; set; }
        public IEnumerable<StreamOptions>? Streams { get; set; }
    }

    public class ProducerOptions
    {
        public string Reference { get; set; } = string.Empty;
        public IEnumerable<StreamOptions>? Streams { get; set; }
    }

    public sealed class StreamOptions
    {
        public string Name { get; set; } = string.Empty;
        public ulong? MaxLengthBytes { get; set; }
        public int? MaxSegmentSizeBytes { get; set; }
        public TimeSpan? MaxAge { get; set; }
    }
}