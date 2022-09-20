using System.Diagnostics.CodeAnalysis;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public sealed class TestHttpClientFactory : IHttpClientFactory
{
    private readonly IDictionary<string, HttpClient> _clients;

    public TestHttpClientFactory() : this(new HttpClient())
    {
    }

    public TestHttpClientFactory(HttpClient client, string name = "")
    {
        _clients = new Dictionary<string, HttpClient>
        {
            [name] = client
        };
    }

    public TestHttpClientFactory(IDictionary<string, HttpClient> clients)
    {
        _clients = clients;
    }
    
    public HttpClient CreateClient(string name)
    {
        if (!_clients.TryGetValue(name, out var client))
        {
            throw new InvalidOperationException($"HTTP Client named: '{name}' was not found.");
        }

        return client;
    }
}