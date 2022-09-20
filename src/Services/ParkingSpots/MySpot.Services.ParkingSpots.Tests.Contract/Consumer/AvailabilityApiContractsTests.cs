using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Micro.HTTP;
using Micro.Testing;
using Microsoft.Extensions.Options;
using MySpot.Services.ParkingSpots.Core.Clients;
using MySpot.Services.ParkingSpots.Core.Clients.DTO;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.ParkingSpots.Tests.Contract.Consumer;

[ExcludeFromCodeCoverage]
public class AvailabilityApiContractsTests : IDisposable
{
    [Fact]
    public async Task given_valid_request_add_resource_should_succeed()
    {
        var dto = new AddResourceDto(Guid.NewGuid(), 1, new[] {"test"});

        _endpointContract.Pact
            .UponReceiving("A valid request for add resource")
            .WithRequest(HttpMethod.Post, "/resources")
            .WithJsonBody(dto)
            .WillRespond()
            .WithStatus(HttpStatusCode.Created);

        await _endpointContract.Pact.VerifyAsync(_ =>
            _apiClient.AddResourceAsync(dto.ResourceId, dto.Capacity, dto.Tags));

        await _endpointContract.PublishToPactBrokerAsync("1");
    }

    #region Arrange

    private readonly EndpointContract _endpointContract;
    private readonly IAvailabilityApiClient _apiClient;

    public AvailabilityApiContractsTests(ITestOutputHelper output)
    {
        _endpointContract = new EndpointContract("ParkingSpots", "Availability", output);
        _apiClient = new AvailabilityApiClient(new TestHttpClientFactory(),
            new OptionsWrapper<HttpClientOptions>(new HttpClientOptions
            {
                Services = new Dictionary<string, string>
                {
                    ["availability"] = $"http://localhost:{_endpointContract.Port}"
                }
            }));
    } 

    #endregion

    public void Dispose()
    {
        _endpointContract.Dispose();
    }
}