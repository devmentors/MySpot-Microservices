namespace Micro.Observability.Tracing;

public sealed class TracingOptions
{
    public bool Enabled { get; set; }
    public string Exporter { get; set; } = string.Empty;
    public JaegerOptions Jaeger { get; set; } = new();

    public sealed class JaegerOptions
    {
        public string AgentHost { get; set; } = "localhost";
        public int AgentPort { get; set; } = 6831;
        public int? MaxPayloadSizeInBytes { get; set; }
        public string ExportProcessorType { get; set; } = "batch";
    }
}