using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Handlers;
using Micro.Testing;
using MySpot.Services.Availability.Application.Commands;
using MySpot.Services.Availability.Application.Commands.Handlers;
using MySpot.Services.Availability.Application.Events;
using MySpot.Services.Availability.Core.Repositories;
using MySpot.Services.Availability.Infrastructure.DAL.Repositories;
using Shouldly;
using Xunit;

namespace MySpot.Services.Availability.Tests.Integration.Commands;

[ExcludeFromCodeCoverage]
public class AddResourceHandlerTests : IDisposable
{
    private Task Act(AddResource command) => _handler.HandleAsync(command);

    [Fact]
    public async Task given_valid_command_adding_resource_should_succeed_and_publish_an_event()
    {
        await _testDatabase.InitAsync();
        var resourceAddedSubscription = _testMessageBroker.SubscribeAsync<ResourceAdded>();

        var command = new AddResource(Guid.NewGuid(), 2, new[] {"test"});

        await Act(command);

        var resource = await _resourcesRepository.GetAsync(command.ResourceId);
        resource.ShouldNotBeNull();
        var resourceAdded = await resourceAddedSubscription;
        resourceAdded.ShouldNotBeNull();
    }

    #region Arrange
    
    private readonly TestDatabase _testDatabase;
    private readonly TestMessageBroker _testMessageBroker;
    private readonly IResourcesRepository _resourcesRepository;
    private readonly ICommandHandler<AddResource> _handler;

    public AddResourceHandlerTests()
    {
        _testDatabase = new TestDatabase();
        _testMessageBroker = new TestMessageBroker();
        _resourcesRepository = new ResourcesRepository(_testDatabase.Context);
        _handler = new AddResourceHandler(_resourcesRepository, _testMessageBroker.MessageBroker);
    }
    
    #endregion

    public void Dispose()
    {
        _testDatabase.Dispose();
        _testMessageBroker.Dispose();
    } 
}