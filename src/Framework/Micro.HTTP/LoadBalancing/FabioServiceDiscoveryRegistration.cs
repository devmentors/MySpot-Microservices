using Micro.HTTP.ServiceDiscovery;
using Microsoft.Extensions.Options;

namespace Micro.HTTP.LoadBalancing;

internal sealed class FabioServiceDiscoveryRegistration : IServiceDiscoveryRegistration
{
    public IEnumerable<string> Tags { get; }

    public FabioServiceDiscoveryRegistration(IOptions<ConsulOptions> options)
    {
        var serviceName = options.Value.Service.Name;
        Tags = new[] {$"urlprefix-/{serviceName} strip=/{serviceName}"};
    }
}