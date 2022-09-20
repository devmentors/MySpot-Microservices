using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Core.ValueObjects;
using MySpot.Services.Availability.Infrastructure.DAL.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Availability.Tests.Contract.Provider;

[ExcludeFromCodeCoverage]
[Collection(Const.TestCollection)]
public class ReservationsConsumerContractsTests : IDisposable
{
    [Fact]
    public async Task should_honour_get_resource_pact_with_reservations_consumer()
    {
        var pactFile = _endpointContract.GetPactFile();
        var resourceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        await _testServer.StartAsync();
        await _resourcesRepository.AddAsync(Resource.Create(resourceId, 2, new[] { new Tag("test") }));
        
        _endpointContract.Verifier
            .ServiceProvider(_endpointContract.Provider, _testServer.Url)
            .WithFileSource(pactFile)
            .WithSslVerificationDisabled()
            .Verify();
    }

    #region Arrange
    
    private readonly TestDatabase _testDatabase;
    private readonly IResourcesRepository _resourcesRepository;
    private readonly EndpointContract _endpointContract;
    private readonly TestServer _testServer;

    public ReservationsConsumerContractsTests(ITestOutputHelper output)
    {
        _testDatabase = new TestDatabase();
        _resourcesRepository = new ResourcesRepository(_testDatabase.Context);
        _endpointContract = new EndpointContract("Reservations", "Availability", output);
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