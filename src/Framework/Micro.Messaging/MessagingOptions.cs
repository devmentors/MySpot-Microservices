namespace Micro.Messaging;

public sealed class MessagingOptions
{
    public ResiliencyOptions Resiliency { get; set; } = new();

    public sealed class ResiliencyOptions
    {
        public int Retries { get; set; } = 3;
        public TimeSpan? RetryInterval { get; set; }
        public bool Exponential { get; set; }
    }
}