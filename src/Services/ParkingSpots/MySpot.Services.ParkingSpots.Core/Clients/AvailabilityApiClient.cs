using System.Net.Http.Json;
using Micro.HTTP;
using Microsoft.Extensions.Options;
using MySpot.Services.ParkingSpots.Core.Clients.DTO;
using MySpot.Services.ParkingSpots.Core.Exceptions;

namespace MySpot.Services.ParkingSpots.Core.Clients;

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

    public async Task AddResourceAsync(Guid resourceId, int capacity, IEnumerable<string> tags)
    {
        var client = _factory.CreateClient(_clientName);
        var payload = new AddResourceDto(resourceId, capacity, tags);
        var response = await client.PostAsJsonAsync($"{_url}/resources", payload);
        if (!response.IsSuccessStatusCode)
        {
            throw new CannotAddResourceException(resourceId);
        }
    }
}