namespace Micro.Messaging.Azure.ServiceBus;

public sealed class AzureServiceBusOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public bool InitializeResources { get; set; }
}