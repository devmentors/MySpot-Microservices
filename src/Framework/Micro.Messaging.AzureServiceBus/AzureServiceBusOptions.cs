namespace Micro.Messaging.AzureServiceBus;

public sealed class AzureServiceBusOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public bool InitializeResources { get; set; }
}