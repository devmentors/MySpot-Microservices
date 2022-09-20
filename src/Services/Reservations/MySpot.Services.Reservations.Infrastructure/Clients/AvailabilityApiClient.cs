using System.Net.Http.Json;
using Micro.HTTP;
using Microsoft.Extensions.Options;
using MySpot.Services.Reservations.Application.Clients;
using MySpot.Services.Reservations.Application.Clients.DTO;

namespace MySpot.Services.Reservations.Infrastructure.Clients;

internal sealed class AvailabilityApiClient : IAvailabilityApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly string _url;
    private readonly string _clientName;

    public AvailabilityApiClient(IHttpClientFactory factory, IOptions<HttpClientOptions> options)
    {
        _factory = factory;
        _clientName = options.Value.Name;
        _url = options.Value.Services["availability"];
    }

    public async Task<ResourceDto?> GetResourceAsync(Guid resourceId)
    {
        var client = _factory.CreateClient(_clientName);
        return await client.GetFromJsonAsync<ResourceDto>($"{_url}/resources/{resourceId}");
    }
}