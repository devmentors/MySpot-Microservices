namespace Micro.Transactions.Outbox;

public class OutboxMessage
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? SentAt { get; set; }
}