using Micro.HTTP;
using Microsoft.Extensions.Options;

namespace MySpot.Services.Notifications.Api.Clients;

internal sealed class EmailApiClient : IEmailApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly string _url;
    private readonly string _clientName;

    public EmailApiClient(IHttpClientFactory factory, IOptions<HttpClientOptions> options)
    {
        _factory = factory;
        _clientName = options.Value.Name;
        _url = options.Value.Services["email"];
    }

    public async Task SendAsync(string receiver, string title, string body)
    {
        var client = _factory.CreateClient(_clientName);
        await client.PostAsJsonAsync($"{_url}/send-email", new {receiver, title, body});
    }
}