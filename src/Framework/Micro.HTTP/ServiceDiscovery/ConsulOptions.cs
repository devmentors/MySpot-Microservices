namespace Micro.HTTP.ServiceDiscovery;

public sealed class ConsulOptions
{
    public bool Enabled { get; set; }
    public string Url { get; set; } = string.Empty;
    public ServiceRegistration Service { get; set; } = new();
    public HealthCheckRegistration HealthCheck { get; set; } = new();

    public sealed class ServiceRegistration
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public sealed class HealthCheckRegistration
    {
        public string Endpoint { get; set; } = string.Empty;
        public TimeSpan? Interval { get; set; }
        public TimeSpan? DeregisterInterval { get; set; }
    }
}