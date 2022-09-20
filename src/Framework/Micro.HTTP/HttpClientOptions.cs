namespace Micro.HTTP;

public sealed class HttpClientOptions
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public CertificateOptions? Certificate { get; set; }
    public ResiliencyOptions Resiliency { get; set; } = new();
    public RequestMaskingOptions RequestMasking { get; set; } = new();
    public Dictionary<string, string> Services { get; set; } = new();

    public sealed class CertificateOptions
    {
        public string Location { get; set; } = string.Empty;
        public string? Password { get; set; }
    }

    public sealed class ResiliencyOptions
    {
        public int Retries { get; set; } = 3;
        public TimeSpan? RetryInterval { get; set; }
        public bool Exponential { get; set; }
    }

    public class RequestMaskingOptions
    {
        public bool Enabled { get; set; }
        public List<string> UrlParts { get; set; } = new();
        public string MaskTemplate { get; set; } = "***";
    }
}