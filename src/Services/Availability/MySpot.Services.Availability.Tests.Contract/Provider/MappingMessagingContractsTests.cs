using System;
using System.Diagnostics.CodeAnalysis;
using Micro.Testing;
using MySpot.Services.Availability.Application.Commands;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Availability.Tests.Contract.Provider;

[ExcludeFromCodeCoverage]
public class MappingMessagingContractsTests : IDisposable
{
    [Fact]
    public void should_honour_add_resource_command_pact_with_mapping_consumer()
    {
        var pactFile = _messageContract.GetPactFile();
        var resourceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var command = new AddResource(resourceId, 2, new[] { "test" });

        _messageContract
            .MessagingProvider()
            .WithProviderMessages(scenarios =>
            {
                scenarios.Add("add_resource", () => command);
            })
            .WithFileSource(pactFile)
            .Verify();
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public MappingMessagingContractsTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Mapping", "Availability", output);
    }

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}