using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Mapping.Tests.Contract.Consumer;

[ExcludeFromCodeCoverage]
public class ParkingSpotsMessagingTests : IDisposable
{
    [Fact]
    public async Task given_parking_spot_created_event_content_should_be_valid()
    {
        var parkingSpotId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var @event = new ParkingSpotCreated(parkingSpotId);
        
        _messageContract.Pact
            .ExpectsToReceive("parking_spot_created")
            .WithJsonContent(new TypeMatcher(@event))
            .Verify<ParkingSpotCreated>(e => {});
        
        await _messageContract.PublishToPactBrokerAsync("1");
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public ParkingSpotsMessagingTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Mapping", "ParkingSpots", output);
    } 

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}