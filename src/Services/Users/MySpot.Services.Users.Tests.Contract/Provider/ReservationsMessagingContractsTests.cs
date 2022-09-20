using System;
using System.Diagnostics.CodeAnalysis;
using Micro.Testing;
using MySpot.Services.Users.Core.Events;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Users.Tests.Contract.Provider;

[ExcludeFromCodeCoverage]
public class ReservationsMessagingContractsTests : IDisposable
{
    [Fact]
    public void should_honour_signed_up_event_pact_with_reservations_consumer()
    {
        var pactFile = _messageContract.GetPactFile();
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var @event = new SignedUp(userId, "test@myspot.io", "user", "employee");

        _messageContract
            .MessagingProvider()
            .WithProviderMessages(scenarios =>
            {
                scenarios.Add("signed_up", () => @event);
            })
            .WithFileSource(pactFile)
            .Verify();
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public ReservationsMessagingContractsTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Reservations", "Users", output);
    }

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}