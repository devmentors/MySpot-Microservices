using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Micro.Testing;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Application.DTO;
using MySpot.Services.Availability.Application.Events;
using Shouldly;
using Xunit;

namespace MySpot.Services.Availability.Tests.EndToEnd.Messaging;

[ExcludeFromCodeCoverage]
[Collection(Const.TestCollection)]
public class AddResourceTests : IDisposable
{
    [Fact]
    public async Task add_resource_message_should_create_resource_and_publish_an_event()
    {
        var resourceAddedSubscription = _testMessageBroker.SubscribeAsync<ResourceAdded>();
        var command = new AddResource(Guid.NewGuid(), 2, new[] {"test"});
        
        await _testMessageBroker.MessageBroker.SendAsync(command);
        
        var resourceAdded = await resourceAddedSubscription;
        resourceAdded.ShouldNotBeNull();
        var resource = await _app.Client.GetFromJsonAsync<ResourceDetailsDto>($"resources/{command.ResourceId}");
        resource.ShouldNotBeNull();
    }
    
    #region Arrange
    
    private readonly TestDatabase _testDatabase;
    private readonly TestMessageBroker _testMessageBroker;
    private readonly TestApp<Program> _app;

    public AddResourceTests()
    {
        _testDatabase = new TestDatabase();
        _testMessageBroker = new TestMessageBroker();
        _app = new TestApp<Program>();
    }
    
    #endregion

    public void Dispose()
    {
        _testDatabase.Dispose();
        _testMessageBroker.Dispose();
    }
}