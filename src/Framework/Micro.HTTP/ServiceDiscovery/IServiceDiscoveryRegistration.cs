namespace Micro.HTTP.ServiceDiscovery;

public interface IServiceDiscoveryRegistration
{
    IEnumerable<string> Tags { get; }
}