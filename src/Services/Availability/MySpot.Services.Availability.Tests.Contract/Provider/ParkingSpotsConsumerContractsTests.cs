using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Testing;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Availability.Tests.Contract.Provider;

[ExcludeFromCodeCoverage]
[Collection(Const.TestCollection)]
public class ParkingSpotsConsumerContractsTests : IDisposable
{
    [Fact]
    public async Task should_honour_add_resource_pact_with_parking_spots_consumer()
    {
        var pactFile = _endpointContract.GetPactFile();
        await _testServer.StartAsync();
        
        _endpointContract.Verifier
            .ServiceProvider(_endpointContract.Provider, _testServer.Url)
            .WithFileSource(pactFile)
            .WithSslVerificationDisabled()
            .Verify();
    }

    #region Arrange

    private readonly TestDatabase _testDatabase;
    private readonly EndpointContract _endpointContract;
    private readonly TestServer _testServer;

    public ParkingSpotsConsumerContractsTests(ITestOutputHelper output)
    {
        _testDatabase = new TestDatabase();
        _endpointContract = new EndpointContract("ParkingSpots", "Availability", output);
        _testServer = new TestServer("MySpot.Services.Availability.Api", output);
    }

    #endregion

    public void Dispose()
    {
        _endpointContract.Dispose();
        _testServer.Dispose();
        _testDatabase.Dispose();
    }
}