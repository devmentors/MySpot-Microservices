namespace Micro.Observability.Azure;

public sealed class ApplicationInsightsOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}