namespace Micro.Observability.Metrics;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MeterAttribute : Attribute
{
    public string Key { get; }

    public MeterAttribute(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Observability key cannot be empty.");
        }
            
        Key = key;
    }
}