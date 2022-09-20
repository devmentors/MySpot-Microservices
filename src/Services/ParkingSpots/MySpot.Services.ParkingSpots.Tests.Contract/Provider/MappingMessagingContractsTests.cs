using System;
using System.Diagnostics.CodeAnalysis;
using Micro.Testing;
using MySpot.Services.ParkingSpots.Core.Events;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.ParkingSpots.Tests.Contract.Provider;

[ExcludeFromCodeCoverage]
public class MappingMessagingContractsTests : IDisposable
{
    [Fact]
    public void should_honour_parking_spot_created_event_pact_with_mapping_consumer()
    {
        var pactFile = _messageContract.GetPactFile();
        var parkingSpotId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var @event = new ParkingSpotCreated(parkingSpotId);

        _messageContract
            .MessagingProvider()
            .WithProviderMessages(scenarios =>
            {
                scenarios.Add("parking_spot_created", () => @event);
            })
            .WithFileSource(pactFile)
            .Verify();
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public MappingMessagingContractsTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Mapping", "ParkingSpots", output);
    }

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}