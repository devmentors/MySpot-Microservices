namespace Micro.API.Networking;

public sealed class NetworkingOptions
{
    public bool Enabled { get; set; }
    public List<KnownNetwork> Networks { get; set; } = new();

    public class KnownNetwork
    {
        public string Prefix { get; set; } = string.Empty;
        public int PrefixLength { get; set; }
    }
}