using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit.Abstractions;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public sealed class PactBrokerPublisher
{
    private const string RequestHeader = "Pact-Requester";
    private const string JsonContentType = "application/json";
    private readonly string _consumer;
    private readonly string _provider;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string _pactBrokerUrl;
    private readonly string _endpoint;
    private readonly string _apiKey;

    public PactBrokerPublisher(string consumer, string provider, ITestOutputHelper testOutputHelper,
        string pactBrokerUrl = "http://localhost:9292", string apiKey = "")
    {
        _consumer = consumer;
        _provider = provider;
        _testOutputHelper = testOutputHelper;
        _pactBrokerUrl = pactBrokerUrl;
        _apiKey = apiKey;
        _endpoint = $"pacts/provider/{provider}/consumer/{consumer}/version";
    }

    public async Task PublishAsync(string pact, string version)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_pactBrokerUrl}/{_endpoint}/{version}"),
            Content = new StringContent(pact, Encoding.UTF8, JsonContentType),
            Method = HttpMethod.Put
        };

        if (!string.IsNullOrWhiteSpace(_apiKey))
        {
            request.Headers.Add(RequestHeader, _apiKey);
        }

        try
        {
            _testOutputHelper.WriteLine($"Publishing a pact with version: '{version}' " +
                                        $"[{_consumer} -> {_provider}] to pact broker...");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Couldn't publish a pact with version: '{version}' " +
                                                    $"[{_consumer} -> {_provider}] to pact broker.");
            }

            _testOutputHelper.WriteLine($"Published a pact with version: '{version}' " +
                                        $"[{_consumer} -> {_provider}] to pact broker.");
        }
        catch (Exception exception)
        {
            _testOutputHelper.WriteLine(exception.ToString());
            throw;
        }
    }
}