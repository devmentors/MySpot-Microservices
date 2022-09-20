using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Commands;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Mapping.Tests.Contract.Consumer;

[ExcludeFromCodeCoverage]
public class AvailabilityMessagingTests : IDisposable
{
    [Fact]
    public async Task given_add_resource_command_content_should_be_valid()
    {
        var resourceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var command = new AddResource(resourceId, 2, new[] { "test" });
        
        _messageContract.Pact
            .ExpectsToReceive("add_resource")
            .WithJsonContent(new TypeMatcher(command))
            .Verify<AddResource>(e => {});
        
        await _messageContract.PublishToPactBrokerAsync("1");
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public AvailabilityMessagingTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Mapping", "Availability", output);
    } 

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}