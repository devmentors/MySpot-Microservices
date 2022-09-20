using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Micro.HTTP;
using Micro.Testing;
using Microsoft.Extensions.Options;
using MySpot.Services.Reservations.Application.Clients;
using MySpot.Services.Reservations.Application.Clients.DTO;
using MySpot.Services.Reservations.Infrastructure.Clients;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Reservations.Tests.Contract.Consumer;

[ExcludeFromCodeCoverage]
public class AvailabilityApiContractsTests : IDisposable
{
    [Fact]
    public async Task given_valid_request_get_resource_should_succeed()
    {
        var resourceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var dto = new ResourceDto(resourceId, 2, new List<string> { "test" });

        _endpointContract.Pact
            .UponReceiving("A valid request for get resource")
            .WithRequest(HttpMethod.Get, $"/resources/{resourceId}")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithJsonBody(new TypeMatcher(dto));

        await _endpointContract.Pact.VerifyAsync(_ => _apiClient.GetResourceAsync(resourceId));
        
        await _endpointContract.PublishToPactBrokerAsync("1");
    }

    #region Arrange

    private readonly EndpointContract _endpointContract;
    private readonly IAvailabilityApiClient _apiClient;

    public AvailabilityApiContractsTests(ITestOutputHelper output)
    {
        _endpointContract = new EndpointContract("Reservations", "Availability", output);
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