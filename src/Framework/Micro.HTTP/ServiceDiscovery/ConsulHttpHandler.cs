using Consul;
using Consul.Filtering;
using Microsoft.Extensions.Options;

namespace Micro.HTTP.ServiceDiscovery;

internal sealed class ConsulHttpHandler : DelegatingHandler
{
    private const string HostDockerInternal = "host.docker.internal";
    private readonly StringFieldSelector _selector = new("Service");
    private readonly IConsulClient _client;
    private readonly bool _enabled;

    public ConsulHttpHandler(IConsulClient client, IOptions<ConsulOptions> options)
    {
        _client = client;
        _enabled = options.Value.Enabled;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!_enabled)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        if (request.RequestUri is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }
        
        var serviceName = request.RequestUri.Host;
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var filter = _selector == serviceName;
        var services = await _client.Agent.Services(filter, cancellationToken);
        if (!services.Response.Any())
        {
            throw new ServiceNotFoundException(serviceName);
        }

        var service = services.Response.First().Value;
        if (service.Address.Contains(HostDockerInternal))
        {
            service.Address = service.Address.Replace(HostDockerInternal, "localhost");
        }
        
        var uriBuilder = new UriBuilder(request.RequestUri)
        {
            Host = service.Address,
            Port = service.Port
        };
        
        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}