using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Reservations.Application.Events.External;
using PactNet.Matchers;
using Xunit;
using Xunit.Abstractions;

namespace MySpot.Services.Reservations.Tests.Contract.Consumer;

[ExcludeFromCodeCoverage]
public class UsersMessagingTests : IDisposable
{
    [Fact]
    public async Task given_signed_up_event_content_should_be_valid()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var @event = new SignedUp(userId, "employee");
        
        _messageContract.Pact
            .ExpectsToReceive("signed_up")
            .WithJsonContent(new TypeMatcher(@event))
            .Verify<SignedUp>(e => {});
        
        await _messageContract.PublishToPactBrokerAsync("1");
    }

    #region Arrange

    private readonly MessageContract _messageContract;

    public UsersMessagingTests(ITestOutputHelper output)
    {
        _messageContract = new MessageContract("Reservations", "Users", output);
    } 

    #endregion

    public void Dispose()
    {
        _messageContract.Dispose();
    }
}